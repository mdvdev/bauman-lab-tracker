using LabTracker.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LabTracker.Infrastructure.Persistence.Converters;

public class NameConverter : ValueConverter<Name, string>
{
    public NameConverter()
        : base(
            v => v.Value,
            v => new Name(v)
        )
    {
    }
}