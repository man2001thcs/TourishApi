-- DROP SCHEMA dbo;

CREATE SCHEMA dbo;
-- TourishApi_db.dbo.GuestMessageCon definition

-- Drop table

-- DROP TABLE TourishApi_db.dbo.GuestMessageCon;

CREATE TABLE TourishApi_db.dbo.GuestMessageCon (
	Id uniqueidentifier NOT NULL,
	GuestName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT N'' NOT NULL,
	GuestEmail nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT N'' NOT NULL,
	GuestPhoneNumber nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT N'' NOT NULL,
	ConnectionID nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	UserAgent nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Connected bit NOT NULL,
	CreateDate datetime2 DEFAULT getutcdate() NOT NULL,
	isChatWithBot int DEFAULT 0 NOT NULL,
	CONSTRAINT PK_GuestMessageCon PRIMARY KEY (Id)
);


-- TourishApi_db.dbo.MovingContact definition

-- Drop table

-- DROP TABLE TourishApi_db.dbo.MovingContact;

CREATE TABLE TourishApi_db.dbo.MovingContact (
	Id uniqueidentifier NOT NULL,
	BranchName nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	VehicleType int NOT NULL,
	HotlineNumber nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	SupportEmail nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	HeadQuarterAddress nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	DiscountFloat real NOT NULL,
	DiscountAmount float NOT NULL,
	Description nvarchar(900) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CreateDate datetime2 NOT NULL,
	UpdateDate datetime2 NOT NULL,
	CONSTRAINT PK_MovingContact PRIMARY KEY (Id)
);


-- TourishApi_db.dbo.ReqTemporaryToken definition

-- Drop table

-- DROP TABLE TourishApi_db.dbo.ReqTemporaryToken;

CREATE TABLE TourishApi_db.dbo.ReqTemporaryToken (
	Id uniqueidentifier NOT NULL,
	Token nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	IsActivated bit NOT NULL,
	Purpose int NOT NULL,
	CreateDate datetime2 DEFAULT getutcdate() NOT NULL,
	ClosedDate datetime2 NULL,
	CONSTRAINT PK_ReqTemporaryToken PRIMARY KEY (Id)
);


-- TourishApi_db.dbo.RestHouseContact definition

-- Drop table

-- DROP TABLE TourishApi_db.dbo.RestHouseContact;

CREATE TABLE TourishApi_db.dbo.RestHouseContact (
	Id uniqueidentifier NOT NULL,
	PlaceBranch nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	RestHouseType int NOT NULL,
	HotlineNumber nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	SupportEmail nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	HeadQuarterAddress nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	DiscountFloat real NOT NULL,
	DiscountAmount float NOT NULL,
	Description nvarchar(900) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CreateDate datetime2 NOT NULL,
	UpdateDate datetime2 NOT NULL,
	CONSTRAINT PK_RestHouseContact PRIMARY KEY (Id)
);


-- TourishApi_db.dbo.Restaurant definition

-- Drop table

-- DROP TABLE TourishApi_db.dbo.Restaurant;

CREATE TABLE TourishApi_db.dbo.Restaurant (
	Id uniqueidentifier NOT NULL,
	PlaceBranch nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	HotlineNumber nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	SupportEmail nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	HeadQuarterAddress nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	DiscountFloat real NOT NULL,
	DiscountAmount float NOT NULL,
	Description nvarchar(900) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CreateDate datetime2 NOT NULL,
	UpdateDate datetime2 NOT NULL,
	CONSTRAINT PK_Restaurant PRIMARY KEY (Id)
);


-- TourishApi_db.dbo.TourishCategory definition

-- Drop table

-- DROP TABLE TourishApi_db.dbo.TourishCategory;

CREATE TABLE TourishApi_db.dbo.TourishCategory (
	Id uniqueidentifier NOT NULL,
	Name nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Description nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreateDate datetime2 DEFAULT getutcdate() NOT NULL,
	UpdateDate datetime2 NOT NULL,
	CONSTRAINT PK_TourishCategory PRIMARY KEY (Id)
);


-- TourishApi_db.dbo.TourishPlan definition

-- Drop table

-- DROP TABLE TourishApi_db.dbo.TourishPlan;

CREATE TABLE TourishApi_db.dbo.TourishPlan (
	Id uniqueidentifier NOT NULL,
	TourName nvarchar(300) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	StartingPoint nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	EndPoint nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Description ntext COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT '' NULL,
	CreateDate datetime2 DEFAULT getutcdate() NOT NULL,
	UpdateDate datetime2 NOT NULL,
	SupportNumber nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT N'' NOT NULL,
	CONSTRAINT PK_TourishPlan PRIMARY KEY (Id)
);


-- TourishApi_db.dbo.[User] definition

-- Drop table

-- DROP TABLE TourishApi_db.dbo.[User];

CREATE TABLE TourishApi_db.dbo.[User] (
	Id uniqueidentifier NOT NULL,
	UserName nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	PasswordHash nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Role] int NOT NULL,
	Email nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	FullName nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	PhoneNumber nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Address nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CreateDate datetime2 DEFAULT '0001-01-01T00:00:00.0000000' NOT NULL,
	UpdateDate datetime2 DEFAULT '0001-01-01T00:00:00.0000000' NOT NULL,
	PasswordSalt nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT N'' NOT NULL,
	AccessFailedCount int DEFAULT 0 NOT NULL,
	LockoutEnd datetime2 NULL,
	TwoFactorEnabled bit DEFAULT CONVERT([bit],(0)) NOT NULL,
	CONSTRAINT PK_User PRIMARY KEY (Id)
);
 CREATE  UNIQUE NONCLUSTERED INDEX IX_User_UserName ON dbo.User (  UserName ASC  )  
	 WHERE  ([UserName] IS NOT NULL)
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- TourishApi_db.dbo.[__EFMigrationsHistory] definition

-- Drop table

-- DROP TABLE TourishApi_db.dbo.[__EFMigrationsHistory];

CREATE TABLE TourishApi_db.dbo.[__EFMigrationsHistory] (
	MigrationId nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	ProductVersion nvarchar(32) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CONSTRAINT PK___EFMigrationsHistory PRIMARY KEY (MigrationId)
);


-- TourishApi_db.dbo.AdminMessageCon definition

-- Drop table

-- DROP TABLE TourishApi_db.dbo.AdminMessageCon;

CREATE TABLE TourishApi_db.dbo.AdminMessageCon (
	Id uniqueidentifier NOT NULL,
	AdminId uniqueidentifier NOT NULL,
	ConnectionID nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	UserAgent nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Connected bit NOT NULL,
	CreateDate datetime2 DEFAULT getutcdate() NOT NULL,
	CONSTRAINT PK_AdminMessageCon PRIMARY KEY (Id),
	CONSTRAINT FK_User_AdminMessageCon FOREIGN KEY (AdminId) REFERENCES TourishApi_db.dbo.[User](Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_AdminMessageCon_AdminId ON dbo.AdminMessageCon (  AdminId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- TourishApi_db.dbo.EatSchedule definition

-- Drop table

-- DROP TABLE TourishApi_db.dbo.EatSchedule;

CREATE TABLE TourishApi_db.dbo.EatSchedule (
	Id uniqueidentifier NOT NULL,
	TourishPlanId uniqueidentifier NULL,
	PlaceName nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Address nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	SupportNumber nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreateDate datetime2 DEFAULT getutcdate() NOT NULL,
	UpdateDate datetime2 NOT NULL,
	RestaurantId uniqueidentifier DEFAULT '00000000-0000-0000-0000-000000000000' NOT NULL,
	SinglePrice float NULL,
	Name nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CONSTRAINT PK_EatSchedule PRIMARY KEY (Id),
	CONSTRAINT FK_TourishPlan_EatSchedules FOREIGN KEY (TourishPlanId) REFERENCES TourishApi_db.dbo.TourishPlan(Id)
);
 CREATE NONCLUSTERED INDEX IX_EatSchedule_TourishPlanId ON dbo.EatSchedule (  TourishPlanId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- TourishApi_db.dbo.GuestMessage definition

-- Drop table

-- DROP TABLE TourishApi_db.dbo.GuestMessage;

CREATE TABLE TourishApi_db.dbo.GuestMessage (
	Id uniqueidentifier NOT NULL,
	Content nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	IsRead bit NOT NULL,
	IsDeleted bit NOT NULL,
	CreateDate datetime2 DEFAULT getutcdate() NOT NULL,
	UpdateDate datetime2 DEFAULT getutcdate() NOT NULL,
	GuestMessageConId uniqueidentifier NULL,
	AdminMessageConId uniqueidentifier NULL,
	CONSTRAINT PK_GuestMessage PRIMARY KEY (Id),
	CONSTRAINT FK_Admin_MessageCon FOREIGN KEY (AdminMessageConId) REFERENCES TourishApi_db.dbo.AdminMessageCon(Id),
	CONSTRAINT FK_Guest_MessageCon FOREIGN KEY (GuestMessageConId) REFERENCES TourishApi_db.dbo.GuestMessageCon(Id)
);
 CREATE NONCLUSTERED INDEX IX_GuestMessage_AdminMessageConId ON dbo.GuestMessage (  AdminMessageConId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_GuestMessage_GuestMessageConId ON dbo.GuestMessage (  GuestMessageConId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- TourishApi_db.dbo.GuestMessageConHistory definition

-- Drop table

-- DROP TABLE TourishApi_db.dbo.GuestMessageConHistory;

CREATE TABLE TourishApi_db.dbo.GuestMessageConHistory (
	Id uniqueidentifier NOT NULL,
	GuestConId uniqueidentifier NOT NULL,
	AdminConId uniqueidentifier NULL,
	CreateDate datetime2 DEFAULT getutcdate() NOT NULL,
	CloseDate datetime2 DEFAULT '0001-01-01T00:00:00.0000000' NOT NULL,
	CONSTRAINT PK_GuestMessageConHistory PRIMARY KEY (Id),
	CONSTRAINT FK_GuestMessageCon_GuestMessageConHis_Admin FOREIGN KEY (AdminConId) REFERENCES TourishApi_db.dbo.AdminMessageCon(Id),
	CONSTRAINT FK_GuestMessageCon_GuestMessageConHis_Guest FOREIGN KEY (GuestConId) REFERENCES TourishApi_db.dbo.GuestMessageCon(Id)
);
 CREATE  UNIQUE NONCLUSTERED INDEX IX_GuestMessageConHistory_AdminConId ON dbo.GuestMessageConHistory (  AdminConId ASC  )  
	 WHERE  ([AdminConId] IS NOT NULL)
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE  UNIQUE NONCLUSTERED INDEX IX_GuestMessageConHistory_GuestConId ON dbo.GuestMessageConHistory (  GuestConId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- TourishApi_db.dbo.MovingSchedule definition

-- Drop table

-- DROP TABLE TourishApi_db.dbo.MovingSchedule;

CREATE TABLE TourishApi_db.dbo.MovingSchedule (
	Id uniqueidentifier NOT NULL,
	TourishPlanId uniqueidentifier NULL,
	DriverName nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	VehiclePlate nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	PhoneNumber nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	VehicleType int NOT NULL,
	TransportId uniqueidentifier NOT NULL,
	StartingPlace nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	HeadingPlace nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CreateDate datetime2 DEFAULT getutcdate() NOT NULL,
	UpdateDate datetime2 NOT NULL,
	SinglePrice float NULL,
	BranchName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Name nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CONSTRAINT PK_MovingSchedule PRIMARY KEY (Id),
	CONSTRAINT FK_TourishPlan_MovingSchedule FOREIGN KEY (TourishPlanId) REFERENCES TourishApi_db.dbo.TourishPlan(Id)
);
 CREATE NONCLUSTERED INDEX IX_MovingSchedule_TourishPlanId ON dbo.MovingSchedule (  TourishPlanId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- TourishApi_db.dbo.NotificationCon definition

-- Drop table

-- DROP TABLE TourishApi_db.dbo.NotificationCon;

CREATE TABLE TourishApi_db.dbo.NotificationCon (
	Id uniqueidentifier NOT NULL,
	UserId uniqueidentifier NOT NULL,
	ConnectionID nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	UserAgent nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Connected bit NOT NULL,
	CreateDate datetime2 DEFAULT getutcdate() NOT NULL,
	CONSTRAINT PK_NotificationCon PRIMARY KEY (Id),
	CONSTRAINT FK_User_NotificationCon FOREIGN KEY (UserId) REFERENCES TourishApi_db.dbo.[User](Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_NotificationCon_UserId ON dbo.NotificationCon (  UserId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- TourishApi_db.dbo.NotificationFcmToken definition

-- Drop table

-- DROP TABLE TourishApi_db.dbo.NotificationFcmToken;

CREATE TABLE TourishApi_db.dbo.NotificationFcmToken (
	Id uniqueidentifier NOT NULL,
	UserId uniqueidentifier NOT NULL,
	DeviceToken nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CreateDate datetime2 DEFAULT getutcdate() NOT NULL,
	UpdateDate datetime2 DEFAULT getutcdate() NOT NULL,
	CONSTRAINT PK_NotificationFcmToken PRIMARY KEY (Id),
	CONSTRAINT FK_UserCreate_NotificationFcmToken FOREIGN KEY (UserId) REFERENCES TourishApi_db.dbo.[User](Id) ON DELETE CASCADE
);
 CREATE  UNIQUE NONCLUSTERED INDEX IX_NotificationFcmToken_UserId ON dbo.NotificationFcmToken (  UserId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- TourishApi_db.dbo.RefreshToken definition

-- Drop table

-- DROP TABLE TourishApi_db.dbo.RefreshToken;

CREATE TABLE TourishApi_db.dbo.RefreshToken (
	Id uniqueidentifier NOT NULL,
	UserId uniqueidentifier NOT NULL,
	TokenDescription nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	JwtId nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	IsUsed bit NOT NULL,
	IsRevoked bit NOT NULL,
	IssueDate datetime2 NOT NULL,
	ExpiredDate datetime2 NOT NULL,
	CONSTRAINT PK_RefreshToken PRIMARY KEY (Id),
	CONSTRAINT FK_User_RefreshToken FOREIGN KEY (UserId) REFERENCES TourishApi_db.dbo.[User](Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_RefreshToken_UserId ON dbo.RefreshToken (  UserId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- TourishApi_db.dbo.ScheduleRating definition

-- Drop table

-- DROP TABLE TourishApi_db.dbo.ScheduleRating;

CREATE TABLE TourishApi_db.dbo.ScheduleRating (
	Id uniqueidentifier NOT NULL,
	UserId uniqueidentifier NOT NULL,
	ScheduleId uniqueidentifier NOT NULL,
	ScheduleType int NOT NULL,
	Rating int NOT NULL,
	CreateDate datetime2 NOT NULL,
	UpdateDate datetime2 NOT NULL,
	TourishPlanId uniqueidentifier NOT NULL,
	CONSTRAINT PK_ScheduleRating PRIMARY KEY (Id),
	CONSTRAINT FK_ScheduleRating_TourishPlan_TourishPlanId FOREIGN KEY (TourishPlanId) REFERENCES TourishApi_db.dbo.TourishPlan(Id) ON DELETE CASCADE,
	CONSTRAINT FK_User_ScheduleRating FOREIGN KEY (UserId) REFERENCES TourishApi_db.dbo.[User](Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_ScheduleRating_TourishPlanId ON dbo.ScheduleRating (  TourishPlanId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_ScheduleRating_UserId ON dbo.ScheduleRating (  UserId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- TourishApi_db.dbo.StayingSchedule definition

-- Drop table

-- DROP TABLE TourishApi_db.dbo.StayingSchedule;

CREATE TABLE TourishApi_db.dbo.StayingSchedule (
	Id uniqueidentifier NOT NULL,
	TourishPlanId uniqueidentifier NULL,
	PlaceName nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Address nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	SupportNumber nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	RestHouseType int NOT NULL,
	RestHouseBranchId uniqueidentifier NOT NULL,
	CreateDate datetime2 DEFAULT getutcdate() NOT NULL,
	UpdateDate datetime2 NOT NULL,
	SinglePrice float NULL,
	Name nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CONSTRAINT PK_StayingSchedule PRIMARY KEY (Id),
	CONSTRAINT FK_TourishPlan_StayingSchedules FOREIGN KEY (TourishPlanId) REFERENCES TourishApi_db.dbo.TourishPlan(Id)
);
 CREATE NONCLUSTERED INDEX IX_StayingSchedule_TourishPlanId ON dbo.StayingSchedule (  TourishPlanId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- TourishApi_db.dbo.TotalReceipt definition

-- Drop table

-- DROP TABLE TourishApi_db.dbo.TotalReceipt;

CREATE TABLE TourishApi_db.dbo.TotalReceipt (
	TotalReceiptId uniqueidentifier NOT NULL,
	TourishPlanId uniqueidentifier NULL,
	CreatedDate datetime2 DEFAULT getutcdate() NOT NULL,
	UpdateDate datetime2 NOT NULL,
	CompleteDate datetime2 NULL,
	Description nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Status int NOT NULL,
	CONSTRAINT PK_TotalReceipt PRIMARY KEY (TotalReceiptId),
	CONSTRAINT FK_TourishPlan_TotalReceipt FOREIGN KEY (TourishPlanId) REFERENCES TourishApi_db.dbo.TourishPlan(Id) ON DELETE SET NULL
);
 CREATE  UNIQUE NONCLUSTERED INDEX IX_TotalReceipt_TourishPlanId ON dbo.TotalReceipt (  TourishPlanId ASC  )  
	 WHERE  ([TourishPlanId] IS NOT NULL)
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- TourishApi_db.dbo.TotalScheduleReceipt definition

-- Drop table

-- DROP TABLE TourishApi_db.dbo.TotalScheduleReceipt;

CREATE TABLE TourishApi_db.dbo.TotalScheduleReceipt (
	TotalReceiptId uniqueidentifier NOT NULL,
	MovingScheduleId uniqueidentifier NULL,
	StayingScheduleId uniqueidentifier NULL,
	CreatedDate datetime2 DEFAULT getutcdate() NOT NULL,
	UpdateDate datetime2 NOT NULL,
	CompleteDate datetime2 NULL,
	Description nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Status int NOT NULL,
	CONSTRAINT PK_TotalScheduleReceipt PRIMARY KEY (TotalReceiptId),
	CONSTRAINT FK_MovingSchedule_TotalScheduleReceipt FOREIGN KEY (MovingScheduleId) REFERENCES TourishApi_db.dbo.MovingSchedule(Id) ON DELETE SET NULL,
	CONSTRAINT FK_StayingSchedule_TotalScheduleReceipt FOREIGN KEY (StayingScheduleId) REFERENCES TourishApi_db.dbo.StayingSchedule(Id) ON DELETE SET NULL
);
 CREATE  UNIQUE NONCLUSTERED INDEX IX_TotalScheduleReceipt_MovingScheduleId ON dbo.TotalScheduleReceipt (  MovingScheduleId ASC  )  
	 WHERE  ([MovingScheduleId] IS NOT NULL)
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE  UNIQUE NONCLUSTERED INDEX IX_TotalScheduleReceipt_StayingScheduleId ON dbo.TotalScheduleReceipt (  StayingScheduleId ASC  )  
	 WHERE  ([StayingScheduleId] IS NOT NULL)
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- TourishApi_db.dbo.TourishCategoryRelation definition

-- Drop table

-- DROP TABLE TourishApi_db.dbo.TourishCategoryRelation;

CREATE TABLE TourishApi_db.dbo.TourishCategoryRelation (
	Id uniqueidentifier NOT NULL,
	TourishPlanId uniqueidentifier NOT NULL,
	TourishCategoryId uniqueidentifier NOT NULL,
	CONSTRAINT PK_TourishCategoryRelation PRIMARY KEY (Id),
	CONSTRAINT FK_TourishCategory_TourishCategoryRelation FOREIGN KEY (TourishCategoryId) REFERENCES TourishApi_db.dbo.TourishCategory(Id) ON DELETE CASCADE,
	CONSTRAINT FK_TourishPlan_TourishCategoryRelation FOREIGN KEY (TourishPlanId) REFERENCES TourishApi_db.dbo.TourishPlan(Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_TourishCategoryRelation_TourishCategoryId ON dbo.TourishCategoryRelation (  TourishCategoryId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_TourishCategoryRelation_TourishPlanId ON dbo.TourishCategoryRelation (  TourishPlanId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- TourishApi_db.dbo.TourishComment definition

-- Drop table

-- DROP TABLE TourishApi_db.dbo.TourishComment;

CREATE TABLE TourishApi_db.dbo.TourishComment (
	Id uniqueidentifier NOT NULL,
	UserId uniqueidentifier NOT NULL,
	CreateDate datetime2 NOT NULL,
	UpdateDate datetime2 NOT NULL,
	TourishPlanId uniqueidentifier DEFAULT '00000000-0000-0000-0000-000000000000' NOT NULL,
	CONSTRAINT PK_TourishComment PRIMARY KEY (Id),
	CONSTRAINT FK_TourishPlan_TourishComment FOREIGN KEY (TourishPlanId) REFERENCES TourishApi_db.dbo.TourishPlan(Id) ON DELETE CASCADE,
	CONSTRAINT FK_User_TourishComment FOREIGN KEY (UserId) REFERENCES TourishApi_db.dbo.[User](Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_TourishComment_TourishPlanId ON dbo.TourishComment (  TourishPlanId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_TourishComment_UserId ON dbo.TourishComment (  UserId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- TourishApi_db.dbo.TourishInterest definition

-- Drop table

-- DROP TABLE TourishApi_db.dbo.TourishInterest;

CREATE TABLE TourishApi_db.dbo.TourishInterest (
	Id uniqueidentifier NOT NULL,
	TourishPlanId uniqueidentifier NOT NULL,
	UserId uniqueidentifier NOT NULL,
	InterestStatus int NOT NULL,
	UpdateDate datetime2 NOT NULL,
	CreateDate datetime2 DEFAULT '0001-01-01T00:00:00.0000000' NOT NULL,
	CONSTRAINT PK_TourishInterest PRIMARY KEY (Id),
	CONSTRAINT FK_TourishPlan_TourishInterest FOREIGN KEY (TourishPlanId) REFERENCES TourishApi_db.dbo.TourishPlan(Id) ON DELETE CASCADE,
	CONSTRAINT FK_User_TourishInterest FOREIGN KEY (UserId) REFERENCES TourishApi_db.dbo.[User](Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_TourishInterest_TourishPlanId ON dbo.TourishInterest (  TourishPlanId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_TourishInterest_UserId ON dbo.TourishInterest (  UserId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- TourishApi_db.dbo.TourishRating definition

-- Drop table

-- DROP TABLE TourishApi_db.dbo.TourishRating;

CREATE TABLE TourishApi_db.dbo.TourishRating (
	Id uniqueidentifier NOT NULL,
	UserId uniqueidentifier NOT NULL,
	TourishPlanId uniqueidentifier NOT NULL,
	Rating int NOT NULL,
	CreateDate datetime2 NOT NULL,
	UpdateDate datetime2 NOT NULL,
	CONSTRAINT PK_TourishRating PRIMARY KEY (Id),
	CONSTRAINT FK_TourishPlan_TourishRating FOREIGN KEY (TourishPlanId) REFERENCES TourishApi_db.dbo.TourishPlan(Id) ON DELETE CASCADE,
	CONSTRAINT FK_User_TourishRating FOREIGN KEY (UserId) REFERENCES TourishApi_db.dbo.[User](Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_TourishRating_TourishPlanId ON dbo.TourishRating (  TourishPlanId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_TourishRating_UserId ON dbo.TourishRating (  UserId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- TourishApi_db.dbo.TourishSchedule definition

-- Drop table

-- DROP TABLE TourishApi_db.dbo.TourishSchedule;

CREATE TABLE TourishApi_db.dbo.TourishSchedule (
	Id uniqueidentifier NOT NULL,
	TourishPlanId uniqueidentifier NOT NULL,
	PlanStatus int NOT NULL,
	StartDate datetime2 NOT NULL,
	EndDate datetime2 NOT NULL,
	CreateDate datetime2 DEFAULT getutcdate() NOT NULL,
	UpdateDate datetime2 NOT NULL,
	RemainTicket int DEFAULT 20 NOT NULL,
	TotalTicket int DEFAULT 20 NOT NULL,
	CONSTRAINT PK_TourishSchedule PRIMARY KEY (Id),
	CONSTRAINT FK_TourishPlan_TourishSchedule FOREIGN KEY (TourishPlanId) REFERENCES TourishApi_db.dbo.TourishPlan(Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_TourishSchedule_TourishPlanId ON dbo.TourishSchedule (  TourishPlanId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- TourishApi_db.dbo.UserMessage definition

-- Drop table

-- DROP TABLE TourishApi_db.dbo.UserMessage;

CREATE TABLE TourishApi_db.dbo.UserMessage (
	Id uniqueidentifier NOT NULL,
	UserSentId uniqueidentifier NOT NULL,
	UserReceiveId uniqueidentifier NOT NULL,
	GroupId uniqueidentifier NULL,
	Content nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	IsRead bit NOT NULL,
	IsDeleted bit NOT NULL,
	CreateDate datetime2 DEFAULT getutcdate() NOT NULL,
	UpdateDate datetime2 DEFAULT getutcdate() NOT NULL,
	CONSTRAINT PK_UserMessage PRIMARY KEY (Id),
	CONSTRAINT FK_UserCon_UserMessage FOREIGN KEY (UserReceiveId) REFERENCES TourishApi_db.dbo.[User](Id),
	CONSTRAINT FK_User_SentMessage FOREIGN KEY (UserSentId) REFERENCES TourishApi_db.dbo.[User](Id)
);
 CREATE NONCLUSTERED INDEX IX_UserMessage_UserReceiveId ON dbo.UserMessage (  UserReceiveId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_UserMessage_UserSentId ON dbo.UserMessage (  UserSentId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- TourishApi_db.dbo.UserMessageCon definition

-- Drop table

-- DROP TABLE TourishApi_db.dbo.UserMessageCon;

CREATE TABLE TourishApi_db.dbo.UserMessageCon (
	Id uniqueidentifier NOT NULL,
	UserId uniqueidentifier NOT NULL,
	ConnectionID nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	UserAgent nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Connected bit NOT NULL,
	CreateDate datetime2 DEFAULT getutcdate() NOT NULL,
	CONSTRAINT PK_UserMessageCon PRIMARY KEY (Id),
	CONSTRAINT FK_User_MessageCon FOREIGN KEY (UserId) REFERENCES TourishApi_db.dbo.[User](Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_UserMessageCon_UserId ON dbo.UserMessageCon (  UserId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- TourishApi_db.dbo.FullReceipt definition

-- Drop table

-- DROP TABLE TourishApi_db.dbo.FullReceipt;

CREATE TABLE TourishApi_db.dbo.FullReceipt (
	TotalReceiptId uniqueidentifier NOT NULL,
	OriginalPrice float NOT NULL,
	DiscountFloat real NOT NULL,
	DiscountAmount float NOT NULL,
	CompleteDate datetime2 NULL,
	CreatedDate datetime2 DEFAULT getutcdate() NOT NULL,
	Description nvarchar(900) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	GuestName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT N'' NOT NULL,
	Status int DEFAULT 0 NOT NULL,
	UpdateDate datetime2 DEFAULT '0001-01-01T00:00:00.0000000' NOT NULL,
	TotalTicket int DEFAULT 0 NOT NULL,
	Email nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT N'' NOT NULL,
	PhoneNumber nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT N'' NOT NULL,
	TotalChildTicket int DEFAULT 0 NOT NULL,
	TourishScheduleId uniqueidentifier NULL,
	FullReceiptId int IDENTITY(1,1) NOT NULL,
	PaymentId nvarchar(64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CONSTRAINT PK_FullReceipt PRIMARY KEY (FullReceiptId),
	CONSTRAINT FK_FullReceipt_TourishSchedule FOREIGN KEY (TourishScheduleId) REFERENCES TourishApi_db.dbo.TourishSchedule(Id),
	CONSTRAINT FK_TotalReceipt_FullReceipt FOREIGN KEY (TotalReceiptId) REFERENCES TourishApi_db.dbo.TotalReceipt(TotalReceiptId) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_FullReceipt_TotalReceiptId ON dbo.FullReceipt (  TotalReceiptId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_FullReceipt_TourishScheduleId ON dbo.FullReceipt (  TourishScheduleId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- TourishApi_db.dbo.Instruction definition

-- Drop table

-- DROP TABLE TourishApi_db.dbo.Instruction;

CREATE TABLE TourishApi_db.dbo.Instruction (
	Id uniqueidentifier NOT NULL,
	TourishPlanId uniqueidentifier NULL,
	MovingScheduleId uniqueidentifier NULL,
	StayingScheduleId uniqueidentifier NULL,
	InstructionType int NOT NULL,
	Description ntext COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreateDate datetime2 NOT NULL,
	UpdateDate datetime2 NOT NULL,
	CONSTRAINT PK_Instruction PRIMARY KEY (Id),
	CONSTRAINT FK_MovingSchedule_Instruction FOREIGN KEY (MovingScheduleId) REFERENCES TourishApi_db.dbo.MovingSchedule(Id) ON DELETE SET NULL,
	CONSTRAINT FK_StayingSchedule_Instruction FOREIGN KEY (StayingScheduleId) REFERENCES TourishApi_db.dbo.StayingSchedule(Id) ON DELETE SET NULL,
	CONSTRAINT FK_TourishPlan_Instruction FOREIGN KEY (TourishPlanId) REFERENCES TourishApi_db.dbo.TourishPlan(Id) ON DELETE SET NULL
);
 CREATE NONCLUSTERED INDEX IX_Instruction_MovingScheduleId ON dbo.Instruction (  MovingScheduleId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Instruction_StayingScheduleId ON dbo.Instruction (  StayingScheduleId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Instruction_TourishPlanId ON dbo.Instruction (  TourishPlanId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- TourishApi_db.dbo.Notification definition

-- Drop table

-- DROP TABLE TourishApi_db.dbo.Notification;

CREATE TABLE TourishApi_db.dbo.Notification (
	Id uniqueidentifier NOT NULL,
	UserCreateId uniqueidentifier NULL,
	Content nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	IsRead bit NOT NULL,
	IsDeleted bit NOT NULL,
	CreateDate datetime2 DEFAULT getutcdate() NOT NULL,
	UpdateDate datetime2 DEFAULT getutcdate() NOT NULL,
	UserReceiveId uniqueidentifier NULL,
	ContentCode nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	TourishPlanId uniqueidentifier NULL,
	MovingScheduleId uniqueidentifier NULL,
	StayingScheduleId uniqueidentifier NULL,
	IsGenerate bit DEFAULT CONVERT([bit],(0)) NOT NULL,
	CONSTRAINT PK_Notification PRIMARY KEY (Id),
	CONSTRAINT FK_MovingSchedule_Notification FOREIGN KEY (MovingScheduleId) REFERENCES TourishApi_db.dbo.MovingSchedule(Id),
	CONSTRAINT FK_StayingSchedule_Notification FOREIGN KEY (StayingScheduleId) REFERENCES TourishApi_db.dbo.StayingSchedule(Id),
	CONSTRAINT FK_TourishPlan_Notification FOREIGN KEY (TourishPlanId) REFERENCES TourishApi_db.dbo.TourishPlan(Id) ON DELETE CASCADE,
	CONSTRAINT FK_UserCreate_Notification FOREIGN KEY (UserCreateId) REFERENCES TourishApi_db.dbo.[User](Id),
	CONSTRAINT FK_UserReceive_Notification FOREIGN KEY (UserReceiveId) REFERENCES TourishApi_db.dbo.[User](Id)
);
 CREATE NONCLUSTERED INDEX IX_Notification_MovingScheduleId ON dbo.Notification (  MovingScheduleId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Notification_StayingScheduleId ON dbo.Notification (  StayingScheduleId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Notification_TourishPlanId ON dbo.Notification (  TourishPlanId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Notification_UserCreateId ON dbo.Notification (  UserCreateId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Notification_UserReceiveId ON dbo.Notification (  UserReceiveId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- TourishApi_db.dbo.SaveFile definition

-- Drop table

-- DROP TABLE TourishApi_db.dbo.SaveFile;

CREATE TABLE TourishApi_db.dbo.SaveFile (
	Id uniqueidentifier NOT NULL,
	AccessSourceId uniqueidentifier NOT NULL,
	ResourceType int NOT NULL,
	FileType nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CreatedDate datetime2 DEFAULT getutcdate() NOT NULL,
	MessageId uniqueidentifier NULL,
	TourishPlanId uniqueidentifier NULL,
	CONSTRAINT PK_SaveFile PRIMARY KEY (Id),
	CONSTRAINT FK_SaveFile_TourishPlan_TourishPlanId FOREIGN KEY (TourishPlanId) REFERENCES TourishApi_db.dbo.TourishPlan(Id),
	CONSTRAINT FK_SaveFile_UserMessage_MessageId FOREIGN KEY (MessageId) REFERENCES TourishApi_db.dbo.UserMessage(Id)
);
 CREATE NONCLUSTERED INDEX IX_SaveFile_MessageId ON dbo.SaveFile (  MessageId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_SaveFile_TourishPlanId ON dbo.SaveFile (  TourishPlanId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- TourishApi_db.dbo.ScheduleInterest definition

-- Drop table

-- DROP TABLE TourishApi_db.dbo.ScheduleInterest;

CREATE TABLE TourishApi_db.dbo.ScheduleInterest (
	Id uniqueidentifier NOT NULL,
	MovingScheduleId uniqueidentifier NULL,
	StayingScheduleId uniqueidentifier NULL,
	UserId uniqueidentifier NOT NULL,
	InterestStatus int NOT NULL,
	CreateDate datetime2 NOT NULL,
	UpdateDate datetime2 NOT NULL,
	CONSTRAINT PK_ScheduleInterest PRIMARY KEY (Id),
	CONSTRAINT FK_MovingSchedule_ScheduleInterest FOREIGN KEY (MovingScheduleId) REFERENCES TourishApi_db.dbo.MovingSchedule(Id) ON DELETE SET NULL,
	CONSTRAINT FK_StayingSchedule_ScheduleInterest FOREIGN KEY (StayingScheduleId) REFERENCES TourishApi_db.dbo.StayingSchedule(Id) ON DELETE SET NULL,
	CONSTRAINT FK_User_ScheduleInterest FOREIGN KEY (UserId) REFERENCES TourishApi_db.dbo.[User](Id)
);
 CREATE NONCLUSTERED INDEX IX_ScheduleInterest_MovingScheduleId ON dbo.ScheduleInterest (  MovingScheduleId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_ScheduleInterest_StayingScheduleId ON dbo.ScheduleInterest (  StayingScheduleId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_ScheduleInterest_UserId ON dbo.ScheduleInterest (  UserId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- TourishApi_db.dbo.ServiceSchedule definition

-- Drop table

-- DROP TABLE TourishApi_db.dbo.ServiceSchedule;

CREATE TABLE TourishApi_db.dbo.ServiceSchedule (
	Id uniqueidentifier NOT NULL,
	MovingScheduleId uniqueidentifier NULL,
	StayingScheduleId uniqueidentifier NULL,
	Status int NOT NULL,
	StartDate datetime2 NOT NULL,
	EndDate datetime2 NOT NULL,
	CreateDate datetime2 DEFAULT getutcdate() NOT NULL,
	UpdateDate datetime2 NOT NULL,
	RemainTicket int DEFAULT 20 NOT NULL,
	TotalTicket int DEFAULT 20 NOT NULL,
	CONSTRAINT PK_ServiceSchedule PRIMARY KEY (Id),
	CONSTRAINT FK_MovingSchedule_ServiceSchedule FOREIGN KEY (MovingScheduleId) REFERENCES TourishApi_db.dbo.MovingSchedule(Id) ON DELETE SET NULL,
	CONSTRAINT FK_StayingSchedule_ServiceSchedule FOREIGN KEY (StayingScheduleId) REFERENCES TourishApi_db.dbo.StayingSchedule(Id) ON DELETE SET NULL
);
 CREATE NONCLUSTERED INDEX IX_ServiceSchedule_MovingScheduleId ON dbo.ServiceSchedule (  MovingScheduleId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_ServiceSchedule_StayingScheduleId ON dbo.ServiceSchedule (  StayingScheduleId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- TourishApi_db.dbo.FullScheduleReceipt definition

-- Drop table

-- DROP TABLE TourishApi_db.dbo.FullScheduleReceipt;

CREATE TABLE TourishApi_db.dbo.FullScheduleReceipt (
	TotalReceiptId uniqueidentifier NOT NULL,
	ServiceScheduleId uniqueidentifier NULL,
	GuestName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	PhoneNumber nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Email nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	OriginalPrice float NOT NULL,
	TotalTicket int NOT NULL,
	TotalChildTicket int NOT NULL,
	Status int NOT NULL,
	Description nvarchar(900) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedDate datetime2 DEFAULT getutcdate() NOT NULL,
	UpdateDate datetime2 NOT NULL,
	CompleteDate datetime2 NULL,
	DiscountFloat real NOT NULL,
	DiscountAmount float NOT NULL,
	FullReceiptId int IDENTITY(1,1) NOT NULL,
	PaymentId nvarchar(64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CONSTRAINT PK_FullScheduleReceipt PRIMARY KEY (FullReceiptId),
	CONSTRAINT FK_FullScheduleReceipt_ServiceSchedule FOREIGN KEY (ServiceScheduleId) REFERENCES TourishApi_db.dbo.ServiceSchedule(Id) ON DELETE SET NULL,
	CONSTRAINT FK_TotalScheduleReceipt_FullScheduleReceipt FOREIGN KEY (TotalReceiptId) REFERENCES TourishApi_db.dbo.TotalScheduleReceipt(TotalReceiptId) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_FullScheduleReceipt_ServiceScheduleId ON dbo.FullScheduleReceipt (  ServiceScheduleId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_FullScheduleReceipt_TotalReceiptId ON dbo.FullScheduleReceipt (  TotalReceiptId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;