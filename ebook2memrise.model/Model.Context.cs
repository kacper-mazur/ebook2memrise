﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ebook2memrise.model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ebook2memriseEntities : DbContext
    {
        public ebook2memriseEntities()
            : base("name=ebook2memriseEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<words> words { get; set; }
        public virtual DbSet<configuration> configuration { get; set; }
        public virtual DbSet<raw_words> raw_words { get; set; }
        public virtual DbSet<file_raw> file_raw { get; set; }
        public virtual DbSet<users> users { get; set; }
    }
}
