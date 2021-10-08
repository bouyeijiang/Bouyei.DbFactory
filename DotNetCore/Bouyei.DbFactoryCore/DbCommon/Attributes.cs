using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bouyei.DbFactoryCore
{
    public class BaseAttribute : Attribute
    {
        public string Name { get; set; } = string.Empty;

        public AttributeType AttrType { get; set; }

        public BaseAttribute(string Name)
        {
            this.Name = Name;
        }

        public BaseAttribute() { }

        public BaseAttribute(AttributeType attributeType)
        {
            this.AttrType = attributeType;
        }

        public BaseAttribute(string Name,AttributeType attributeType)
        {
            this.Name = Name;
            this.AttrType = attributeType;
        }

        public static string GetName<T>()
        {
            var type = typeof(T);

            var attribute = type.GetCustomAttributes(type, false).FirstOrDefault();

            if (attribute == null)
            {
                return string.Empty;
            }

            var bAttr = ((BaseAttribute)attribute);
            if (bAttr == null) return string.Empty;

            return bAttr.Name;
        }

        public static AttributeType GetAttributeType<T>()
        {
            var type = typeof(T);

            var attribute = type.GetCustomAttributes(type, false).FirstOrDefault();

            if (attribute == null)
            {
                return AttributeType.None;
            }

            var bAttr = ((BaseAttribute)attribute);
            return bAttr.AttrType;
        }
    }

    public class IgnoreAttribute : BaseAttribute
    {
        public IgnoreAttribute(string Name)
            :base(Name,AttributeType.Ignore)
        {
            this.Name = Name;
        }

        public IgnoreAttribute(AttributeType attrType = AttributeType.Ignore)
            : base(attrType)
        { }
    }

    //public class IgnoreWriteAttribute:BaseAttribute
    //{
    //    public IgnoreWriteAttribute(string Name)
    //       : base(Name, AttributeType.IgnoreWrite)
    //    {
    //        this.Name = Name;
    //    }

    //    public IgnoreWriteAttribute()
    //    {
    //        base.AttrType = AttributeType.IgnoreWrite;
    //    }
    //}

    //public class IgnoreReadAttribute:BaseAttribute
    //{
    //    public IgnoreReadAttribute(string Name)
    //      : base(Name, AttributeType.IgnoreRead)
    //    {
    //        this.Name = Name;
    //    }
    //    public IgnoreReadAttribute()
    //    {
    //        base.AttrType = AttributeType.IgnoreRead;
    //    }
    //}

    public class MappedNameAttribute : BaseAttribute
    {
        public MappedNameAttribute(string Name)
         : base(Name, AttributeType.Mapped)
        {
            this.Name = Name;
        }

        public MappedNameAttribute()
            :base(AttributeType.Mapped)
        { }
    }

    [Flags]
    public enum AttributeType:byte
    {
        None=0,
        Ignore=1,
        IgnoreWrite=2,
        IgnoreRead=4,
        Mapped=8
    }
}
