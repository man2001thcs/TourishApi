using Microsoft.EntityFrameworkCore;
using WebApplication1.Data.Authentication;
using WebApplication1.Data.Connection;
using WebApplication1.Data.Receipt;
using WebApplication1.Data.RestaurantPlace;
using WebApplication1.Data.RestHouse;
using WebApplication1.Data.Schedule;
using WebApplication1.Data.Transport;

namespace WebApplication1.Data.DbContextFile
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions options) : base(options)
        {
        }

        #region DbSet
        public DbSet<PlaneAirline> PlaneAirlineList { get; set; }
        public DbSet<PassengerCar> PassengerCarList { get; set; }

        public DbSet<Restaurant> RestaurantList { get; set; }

        public DbSet<Hotel> HotelList { get; set; }
        public DbSet<HomeStay> HomeStayList { get; set; }

        public DbSet<EatSchedule> EatSchedules { get; set; }
        public DbSet<MovingSchedule> MovingSchedules { get; set; }
        public DbSet<StayingSchedule> StayingSchedules { get; set; }

        public DbSet<TourishPlan> TourishPlan { get; set; }
        public DbSet<TotalReceipt> TotalReceiptList { get; set; }
        public DbSet<FullReceipt> FullReceiptList { get; set; }

        //
        public DbSet<Message> Messages { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public DbSet<NotificationCon> NotificationConList { get; set; }
        public DbSet<MessageCon> MessageConList { get; set; }

        public DbSet<SaveFile> SaveFileList { get; set; }

        //
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TourishPlan>(entity =>
            {
                entity.ToTable(nameof(TourishPlan));
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreateDate).IsRequired().HasDefaultValueSql("getutcdate()");

                entity.HasMany(e => e.MovingSchedules)
                .WithOne(e => e.TourishPlan)
                .HasForeignKey(e => e.TourishPlanId)
                .HasConstraintName("FK_TourishPlan_MovingSchedule");

                entity.HasMany(e => e.EatSchedules)
                .WithOne(e => e.TourishPlan)
                .HasForeignKey(e => e.TourishPlanId)
                .HasConstraintName("FK_TourishPlan_EatSchedules");

                entity.HasMany(e => e.StayingSchedules)
                .WithOne(e => e.TourishPlan)
                .HasForeignKey(e => e.TourishPlanId)
                .HasConstraintName("FK_TourishPlan_StayingSchedules");

                entity.HasOne(e => e.TotalReceipt)
                .WithOne(e => e.TourishPlan)
                .HasForeignKey<TotalReceipt>(e => e.TourishPlanId)
                .HasConstraintName("FK_TourishPlan_TotalReceipt");
            });

            modelBuilder.Entity<TotalReceipt>(entity =>
            {
                entity.ToTable(nameof(TotalReceipt));
                entity.HasKey(e => e.TotalReceiptId);
                entity.Property(e => e.CreatedDate).IsRequired().HasDefaultValueSql("getutcdate()");


                entity.HasMany(e => e.FullReceiptList)
               .WithOne(e => e.TotalReceipt)
               .HasForeignKey(e => e.TotalReceiptId)
               .HasConstraintName("FK_TotalReceipt_FullReceipt");
            });



            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.UserName).IsUnique();
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable(nameof(RefreshToken));

                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.User)
                .WithMany(e => e.RefreshTokenList)
                .HasForeignKey(e => e.UserId)
                .HasConstraintName("FK_User_RefreshToken");
            });


            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable(nameof(Message));
                entity.HasKey(message => message.Id);
                entity.Property(message => message.UpdateDate).IsRequired().HasDefaultValueSql("getutcdate()");
                entity.Property(message => message.CreateDate).IsRequired().HasDefaultValueSql("getutcdate()");

                entity.HasOne(e => e.UserSent)
                .WithMany(e => e.MessageSentList)
                .HasForeignKey(e => e.UserSentId)
                .HasConstraintName("FK_User_SentMessage")
                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(e => e.UserReceive)
                .WithMany(e => e.MessageReceiveList)
                .HasForeignKey(e => e.UserReceiveId)
                .HasConstraintName("FK_User_ReceiveMessage")
                .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable(nameof(Notification));
                entity.HasKey(notification => notification.Id);
                entity.Property(notification => notification.UpdateDate).IsRequired().HasDefaultValueSql("getutcdate()");
                entity.Property(notification => notification.CreateDate).IsRequired().HasDefaultValueSql("getutcdate()");

                entity.HasOne(e => e.User)
                .WithMany(e => e.NotificationList)
                .HasForeignKey(e => e.UserId)
                .HasConstraintName("FK_User_Notification");
            });

            modelBuilder.Entity<NotificationCon>(entity =>
            {
                entity.ToTable(nameof(NotificationCon));
                entity.HasKey(notification => notification.Id);
                entity.Property(notification => notification.CreateDate).IsRequired().HasDefaultValueSql("getutcdate()");

                entity.HasOne(e => e.User)
                .WithMany(e => e.NotificationConList)
                .HasForeignKey(e => e.UserId)
                .HasConstraintName("FK_User_NotificationCon");
            });

            modelBuilder.Entity<MessageCon>(entity =>
            {
                entity.ToTable(nameof(MessageCon));
                entity.HasKey(message => message.Id);
                entity.Property(message => message.CreateDate).IsRequired().HasDefaultValueSql("getutcdate()");

                entity.HasOne(e => e.User)
                .WithMany(e => e.MessageConList)
                .HasForeignKey(e => e.UserId)
                .HasConstraintName("FK_User_MessageCon");
            });

            modelBuilder.Entity<SaveFile>(entity =>
            {
                entity.ToTable(nameof(SaveFile));
                entity.Property(saveFile => saveFile.CreatedDate).IsRequired().HasDefaultValueSql("getutcdate()");
            });
        }
    }
}
