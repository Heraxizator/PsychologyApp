using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Domain.Common;

public abstract class Specification<T>
{
    private static readonly ConcurrentDictionary<string, Func<T, bool>> DelegateCache = new();

    private readonly List<string> cacheKey;

    protected Specification()
    {
        this.cacheKey = new List<string> { GetType().Name };
    }

    protected virtual bool Include => true;

    public virtual bool IsSatisfiedBy(T value)
    {
        if (!this.Include)
        {
            return true;
        }

        Func<T, bool> func = DelegateCache.GetOrAdd(
            string.Join(string.Empty, this.cacheKey),
            _ => ToExpression().Compile());

        return func(value);
    }

    public Specification<T> And(Specification<T> specification)
    {
        if (!specification.Include)
        {
            return this;
        }

        this.cacheKey.Add($"{nameof(this.And)}{specification.GetType()}");

        return new BinarySpecification(this, specification, true);
    }

    public Specification<T> Or(Specification<T> specification)
    {
        if (!specification.Include)
        {
            return this;
        }

        this.cacheKey.Add($"{nameof(this.Or)}{specification.GetType()}");

        return new BinarySpecification(this, specification, false);
    }

    public static implicit operator Expression<Func<T, bool>>(Specification<T> specification)
    {
        return specification.Include
                    ? specification.ToExpression()
                    : value => true;
    }

    public abstract Expression<Func<T, bool>> ToExpression();

    private class BinarySpecification : Specification<T>
    {
        private readonly Specification<T> left;
        private readonly Specification<T> right;
        private readonly bool andOperator;

        public BinarySpecification(Specification<T> left, Specification<T> right, bool andOperator)
        {
            this.right = right;
            this.left = left;
            this.andOperator = andOperator;
        }

        public override Expression<Func<T, bool>> ToExpression()
        {
            Expression<Func<T, bool>> leftExpression = this.left;
            Expression<Func<T, bool>> rightExpression = this.right;

            Expression body = this.andOperator
                ? Expression.AndAlso(leftExpression.Body, rightExpression.Body)
                : Expression.OrElse(leftExpression.Body, rightExpression.Body);

            ParameterExpression parameter = Expression.Parameter(typeof(T));
            body = (BinaryExpression)new ParameterReplacer(parameter).Visit(body);

            body = body ?? throw new InvalidOperationException("Binary expression cannot be null.");

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }
    }

    private class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression parameter;

        internal ParameterReplacer(ParameterExpression parameter)
        {
            this.parameter = parameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return base.VisitParameter(this.parameter);
        }
    }
}

