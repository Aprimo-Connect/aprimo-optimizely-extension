using System;
using System.ComponentModel.DataAnnotations;

namespace Aprimo.Opti.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AprimoFieldNameAttribute : ScaffoldColumnAttribute
    {
        public string FieldName { get; set; }

        public AprimoFieldNameAttribute()
             : base(false)
        {
        }

        public AprimoFieldNameAttribute(string fieldName)
            : this()
        {
            this.FieldName = fieldName;
        }
    }
}