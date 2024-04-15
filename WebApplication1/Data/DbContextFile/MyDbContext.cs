using Microsoft.EntityFrameworkCore;
using WebApplication1.Data.Authentication;
using WebApplication1.Data.Chat;
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
        public DbSet<MovingContact> MovingContactList { get; set; }
        public DbSet<Restaurant> RestaurantList { get; set; }
        public DbSet<RestHouseContact> RestHouseContactList { get; set; }
        public DbSet<EatSchedule> EatSchedules { get; set; }
        public DbSet<MovingSchedule> MovingSchedules { get; set; }
        public DbSet<StayingSchedule> StayingSchedules { get; set; }

        public DbSet<TourishPlan> TourishPlan { get; set; }
        public DbSet<TourishCategory> TourishCategories { get; set; }
        public DbSet<TourishCategoryRelation> TourishCategoryRelations { get; set; }
        public DbSet<TourishInterest> TourishInterests { get; set; }
        public DbSet<TourishComment> TourishComments { get; set; }
        public DbSet<TourishRating> TourishRatings { get; set; }
        public DbSet<TotalReceipt> TotalReceiptList { get; set; }
        public DbSet<FullReceipt> FullReceiptList { get; set; }

        //
        public DbSet<UserMessage> UserMessages { get; set; }
        public DbSet<GuestMessage> GuestMessages { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationFcmToken> NotificationFcmTokens { get; set; }
        public DbSet<NotificationCon> NotificationConList { get; set; }
        public DbSet<UserMessageCon> UserMessageConList { get; set; }
        public DbSet<GuestMessageCon> GuestMessageConList { get; set; }
        public DbSet<AdminMessageCon> AdminMessageConList { get; set; }
        public DbSet<GuestMessageConHistory> GuestMessageConHisList { get; set; }
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

                entity.Property(e => e.Description).HasDefaultValueSql("''");

                entity.HasMany(e => e.MovingSchedules)
                .WithOne(e => e.TourishPlan)
                .HasForeignKey(e => e.TourishPlanId).IsRequired(false)
                .HasConstraintName("FK_TourishPlan_MovingSchedule");

                entity.HasMany(e => e.EatSchedules)
                .WithOne(e => e.TourishPlan)
                .HasForeignKey(e => e.TourishPlanId).IsRequired(false)
                .HasConstraintName("FK_TourishPlan_EatSchedules");

                entity.HasMany(e => e.StayingSchedules)
                .WithOne(e => e.TourishPlan)
                .HasForeignKey(e => e.TourishPlanId).IsRequired(false)
                .HasConstraintName("FK_TourishPlan_StayingSchedules");

                entity.HasOne(e => e.TotalReceipt)
                .WithOne(e => e.TourishPlan)
                .HasForeignKey<TotalReceipt>(e => e.TourishPlanId)
                .HasConstraintName("FK_TourishPlan_TotalReceipt");

                entity.HasMany(e => e.NotificationList)
                .WithOne(e => e.TourishPlan)
                .HasForeignKey(e => e.TourishPlanId)
                .HasConstraintName("FK_TourishPlan_Notification");
            });

            modelBuilder.Entity<MovingSchedule>(entity =>
            {
                entity.ToTable(nameof(MovingSchedule));
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreateDate).IsRequired().HasDefaultValueSql("getutcdate()");

                entity.Property(e => e.Description).HasDefaultValueSql("''");
            });

            modelBuilder.Entity<EatSchedule>(entity =>
            {
                entity.ToTable(nameof(EatSchedule));
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreateDate).IsRequired().HasDefaultValueSql("getutcdate()");

                entity.Property(e => e.Description).HasDefaultValueSql("''");
            });

            modelBuilder.Entity<StayingSchedule>(entity =>
            {
                entity.ToTable(nameof(StayingSchedule));
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreateDate).IsRequired().HasDefaultValueSql("getutcdate()");

                entity.Property(e => e.Description).HasDefaultValueSql("''");
            });

            modelBuilder.Entity<TourishCategory>(entity =>
            {
                entity.ToTable(nameof(TourishCategory));
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreateDate).IsRequired().HasDefaultValueSql("getutcdate()");
            });

            modelBuilder.Entity<TourishCategoryRelation>(entity =>
            {
                entity.ToTable(nameof(TourishCategoryRelation));
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.TourishPlan)
                .WithMany(e => e.TourishCategoryRelations)
                .HasForeignKey(e => e.TourishPlanId)
                .HasConstraintName("FK_TourishPlan_TourishCategoryRelation");

                entity.HasOne(e => e.TourishCategory)
                .WithMany(e => e.TourishCategoryRelations)
                .HasForeignKey(e => e.TourishCategoryId)
                .HasConstraintName("FK_TourishCategory_TourishCategoryRelation");
            });

            modelBuilder.Entity<TourishInterest>(entity =>
            {
                entity.ToTable(nameof(TourishInterest));
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.TourishPlan)
                .WithMany(e => e.TourishInterestList)
                .HasForeignKey(e => e.TourishPlanId)
                .HasConstraintName("FK_TourishPlan_TourishInterest");

                entity.HasOne(e => e.User)
                .WithMany(e => e.TourishInterests)
                .HasForeignKey(e => e.UserId)
                .HasConstraintName("FK_User_TourishInterest");
            });

            modelBuilder.Entity<TourishComment>(entity =>
            {
                entity.ToTable(nameof(TourishComment));
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.User)
                .WithMany(e => e.TourishCommentList)
                .HasForeignKey(e => e.UserId)
                .HasConstraintName("FK_User_TourishComment");

                entity.HasOne(e => e.TourishPlan)
               .WithMany(e => e.TourishCommentList)
               .HasForeignKey(e => e.TourishPlanId)
               .HasConstraintName("FK_TourishPlan_TourishComment");
            });

            modelBuilder.Entity<TourishRating>(entity =>
            {
                entity.ToTable(nameof(TourishRating));
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.User)
                .WithMany(e => e.TourishRatingList)
                .HasForeignKey(e => e.UserId)
                .HasConstraintName("FK_User_TourishRating");

                entity.HasOne(e => e.TourishPlan)
               .WithMany(e => e.TourishRatingList)
               .HasForeignKey(e => e.TourishPlanId)
               .HasConstraintName("FK_TourishPlan_TourishRating");
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


            modelBuilder.Entity<UserMessage>(entity =>
            {
                entity.ToTable(nameof(UserMessage));
                entity.HasKey(message => message.Id);
                entity.Property(message => message.UpdateDate).IsRequired().HasDefaultValueSql("getutcdate()");
                entity.Property(message => message.CreateDate).IsRequired().HasDefaultValueSql("getutcdate()");

                entity.HasOne(e => e.UserSent)
                .WithMany(e => e.UserMessageSentList)
                .HasForeignKey(e => e.UserSentId)
                .HasConstraintName("FK_User_SentMessage")
                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(e => e.UserReceive)
                .WithMany(e => e.UserMessageReceiveList)
                .HasForeignKey(e => e.UserReceiveId)
                .HasConstraintName("FK_UserCon_UserMessage")
                .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<GuestMessage>(entity =>
            {
                entity.ToTable(nameof(GuestMessage));
                entity.HasKey(message => message.Id);
                entity.Property(message => message.UpdateDate).IsRequired().HasDefaultValueSql("getutcdate()");
                entity.Property(message => message.CreateDate).IsRequired().HasDefaultValueSql("getutcdate()");

                entity.HasOne(e => e.GuestMessageCon)
                .WithMany(e => e.GuestMessages)
                .HasForeignKey(e => e.GuestMessageConId)
                .HasConstraintName("FK_GuestCon_GuestMessage").IsRequired(false)
                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(e => e.AdminMessageCon)
                .WithMany(e => e.GuestMessages)
                .HasForeignKey(e => e.AdminMessageConId)
                .HasConstraintName("FK_AdminCon_GuestMessage").IsRequired(false)
                .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable(nameof(Notification));
                entity.HasKey(notification => notification.Id);
                entity.Property(notification => notification.UpdateDate).IsRequired().HasDefaultValueSql("getutcdate()");
                entity.Property(notification => notification.CreateDate).IsRequired().HasDefaultValueSql("getutcdate()");

                entity.HasOne(e => e.UserCreator)
                .WithMany(e => e.NotificationCreateList)
                .HasForeignKey(e => e.UserCreateId).IsRequired(false)
                .HasConstraintName("FK_UserCreate_Notification");

                entity.HasOne(e => e.UserReceiver)
               .WithMany(e => e.NotificationReceiveList)
               .HasForeignKey(e => e.UserReceiveId).IsRequired(false)
               .HasConstraintName("FK_UserReceive_Notification");
            });

            modelBuilder.Entity<NotificationFcmToken>(entity =>
            {
                entity.ToTable(nameof(NotificationFcmToken));
                entity.HasKey(notification => notification.Id);
                entity.Property(notification => notification.UpdateDate).IsRequired().HasDefaultValueSql("getutcdate()");
                entity.Property(notification => notification.CreateDate).IsRequired().HasDefaultValueSql("getutcdate()");

                entity.HasOne(e => e.User)
                .WithOne(e => e.FcmToken)
                .HasForeignKey<NotificationFcmToken>(e => e.UserId)
                .HasConstraintName("FK_UserCreate_NotificationFcmToken");

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

            modelBuilder.Entity<UserMessageCon>(entity =>
            {
                entity.ToTable(nameof(UserMessageCon));
                entity.HasKey(message => message.Id);
                entity.Property(message => message.CreateDate).IsRequired().HasDefaultValueSql("getutcdate()");

                entity.HasOne(e => e.User)
                .WithMany(e => e.UserMessageConList)
                .HasForeignKey(e => e.UserId)
                .HasConstraintName("FK_User_MessageCon");
            });

            modelBuilder.Entity<GuestMessageCon>(entity =>
            {
                entity.ToTable(nameof(GuestMessageCon));
                entity.HasKey(message => message.Id);
                entity.Property(message => message.CreateDate).IsRequired().HasDefaultValueSql("getutcdate()");

                entity.HasMany(e => e.GuestMessages)
                .WithOne(e => e.GuestMessageCon)
                .HasForeignKey(e => e.GuestMessageConId)
                .HasConstraintName("FK_Guest_MessageCon");

            });

            modelBuilder.Entity<AdminMessageCon>(entity =>
            {
                entity.ToTable(nameof(AdminMessageCon));
                entity.HasKey(message => message.Id);
                entity.Property(message => message.CreateDate).IsRequired().HasDefaultValueSql("getutcdate()");

                entity.HasOne(e => e.Admin)
                .WithMany(e => e.AdminMessageConList)
                .HasForeignKey(e => e.AdminId)
                .HasConstraintName("FK_User_AdminMessageCon");

                entity.HasMany(e => e.GuestMessages)
               .WithOne(e => e.AdminMessageCon)
               .HasForeignKey(e => e.AdminMessageConId)
               .HasConstraintName("FK_Admin_MessageCon");


            });

            modelBuilder.Entity<GuestMessageConHistory>(entity =>
            {
                entity.ToTable(nameof(GuestMessageConHistory));
                entity.HasKey(message => message.Id);
                entity.Property(message => message.CreateDate).IsRequired().HasDefaultValueSql("getutcdate()");

                // Remove the existing relationship with AdminCon
                entity.HasOne(e => e.AdminCon)
                    .WithOne(e => e.GuestMessageConHis)
                    .HasForeignKey<GuestMessageConHistory>(e => e.AdminConId).IsRequired(false)
                    .HasConstraintName("FK_GuestMessageCon_GuestMessageConHis_Admin").OnDelete(DeleteBehavior.ClientSetNull);

                // Define the new relationship with GuestCon
                entity.HasOne(e => e.GuestCon)
                    .WithOne(e => e.GuestMessageConHis)
                    .HasForeignKey<GuestMessageConHistory>(e => e.GuestConId)
                    .HasConstraintName("FK_GuestMessageCon_GuestMessageConHis_Guest").OnDelete(DeleteBehavior.ClientSetNull); ;
            });

            modelBuilder.Entity<SaveFile>(entity =>
            {
                entity.ToTable(nameof(SaveFile));
                entity.Property(saveFile => saveFile.CreatedDate).IsRequired().HasDefaultValueSql("getutcdate()");
            });
        }
    }
}
