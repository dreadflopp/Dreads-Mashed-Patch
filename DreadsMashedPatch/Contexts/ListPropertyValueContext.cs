using System;
using System.Collections.Generic;
using DreadsMashedPatch.Contexts.Interfaces;

namespace DreadsMashedPatch.Contexts
{
    public class ListPropertyValueContext<T>(T item, string ownerMod) : IPropertyValueContext<T>
    {
        public T Value { get; set; } = item;
        public string OwnerMod { get; set; } = ownerMod;
        public bool IsRemoved { get; set; } = false;
        public int? AlignmentRowId { get; set; }
        public string? AlignmentOwnerMod { get; set; } = ownerMod;
    }
}
