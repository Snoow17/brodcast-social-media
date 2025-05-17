﻿using BrodcastSocialMedia.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace BrodcastSocialMedia.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        public DbSet<Broadcast> Broadcasts { get; set; }

        public DbSet<BroadcastLike> BroadcastLikes { get; set; }

    }
}
