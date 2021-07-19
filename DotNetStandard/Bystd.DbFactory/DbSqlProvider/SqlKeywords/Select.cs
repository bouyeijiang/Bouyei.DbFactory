﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bystd.DbFactory.DbSqlProvider.SqlKeywords
{
    using SqlFunctions;

    public class Select<T> : WordsBase
    {
        private string funString = string.Empty;
        private string topString = string.Empty;

        public Select() : base(typeof(T))
        {

        }

        public Select(Count input) : this()
        {
            funString = input.ToString();
        }

        public Select(Avg input) : this()
        {
            funString = input.ToString();
        }

        public Select(Max input) : this()
        {
            funString = input.ToString();
        }

        public Select(Min input) : this()
        {
            funString = input.ToString();
        }

        public Select(Sum input) : this()
        {
            funString = input.ToString();
        }

        public override string ToString()
        {
            if (funString != string.Empty)
            {
                return string.Format("Select {0} ", funString);
            }
            else
            {
                var ColumnNames = GetColumns();
                return string.Format("Select {0}{1} ", topString, base.ToString(ColumnNames));
            }
        }
    }

    public class Select : WordsBase
    {
        public string[] ColumnNames { get; private set; }

        private string topString = string.Empty;
        private string funString = string.Empty;

        public Select() { }

        public Select(params string[] columnNames)
        {
            this.ColumnNames = columnNames;
        }

        public Select(Count input) : this()
        {
            funString = input.ToString();
        }

        public Select(Avg input) : this()
        {
            funString = input.ToString();
        }

        public Select(Max input) : this()
        {
            funString = input.ToString();
        }

        public Select(Min input) : this()
        {
            funString = input.ToString();
        }

        public Select(Sum input) : this()
        {
            funString = input.ToString();
        }

        public override string ToString()
        {
            if (funString != string.Empty)
            {
                return string.Format("Select {0} ", funString);
            }
            else
            {
                return string.Format("Select {0}{1} ", topString, base.ToString(ColumnNames));
            }
        }
    }
}
