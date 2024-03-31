using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Domain.Common;

public static class AbstractMapper<TObject, QObject>
{
    // From the TObject type to the QObject type Configuration

    private static readonly MapperConfiguration configA =
        new(cfg => cfg.CreateMap<TObject, QObject>());

    // From the TObject type to the QObject type Mapper

    public static readonly Mapper MapperA = new(configA);

    // From the QObject type to the TObject type Configuration

    private static readonly MapperConfiguration configB =
        new(cfg => cfg.CreateMap<QObject, TObject>());

    // From the QObject type to the TObject type Mapper

    public static readonly Mapper MapperB = new(configB);
}
