﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Bouyei.DbFactory.DbEntityProvider;
using Bouyei.DbFactory;
using Bouyei.DbFactory.DbMapper;
using System;

namespace Bouyei.DbEntities
{
    [Table("db_user")]
    [MappedName("db_user")]
    public class User : BaseEntity<User>
    {
        public User()
        {

        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public string uname { get; set; }

        public int uage { get; set; }
        public float score { get; set; }
        [Ignore(AttributeType.IgnoreWrite)]
        public DateTime gentime { get; set; }
    }

    public class BaseEntity<T> : TableMapper<T> where T : class
    {
        public BaseEntity()
        {
            string connstr= "Host=127.0.0.1;Port=5432;User id=postgres;Password=bouyei;Database=postgres;";
            var provider = AdoProvider.CreateProvider(connstr, FactoryType.PostgreSQL);
            Initialized(provider);
        }
    }
}
