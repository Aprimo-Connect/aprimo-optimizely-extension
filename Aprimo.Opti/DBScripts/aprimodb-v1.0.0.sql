IF NOT EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[dbo].[tblAprimoPersistantAsset]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[tblAprimoPersistantAsset](
		[Id] [int] NOT NULL,
		[AssetId] [uniqueidentifier] NOT NULL,
		[RenditionId] [varchar](50) NOT NULL,
		[Title] [varchar](max) NULL,
		[CDNUrl] [varchar](max) NULL,
		[ThumbnailUrl] [varchar](max) NULL,
		[MetaInformation] [varchar](max) NULL,
		[Extension] [varchar](10) NULL,
		[ModifiedDate] [datetime] NULL,
	 CONSTRAINT [PK_tblAprimoPersistantAsset] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

CREATE OR ALTER PROCEDURE [dbo].[AprimoDeleteAllAssets]
AS
BEGIN
	TRUNCATE TABLE [dbo].[tblAprimoPersistantAsset]
END
GO

CREATE OR ALTER PROCEDURE [dbo].[AprimoDeleteAsset]
	@Id int
AS
BEGIN
	DELETE FROM [dbo].[tblAprimoPersistantAsset]
	WHERE Id=@Id
END
GO

CREATE OR ALTER PROCEDURE [dbo].[AprimoGetAllAssets]
AS
BEGIN

	SET NOCOUNT ON;

	SELECT
		Id, AssetId, RenditionId, Title, CDNUrl, ThumbnailUrl, MetaInformation, Extension, ModifiedDate
	FROM 
		[dbo].[tblAprimoPersistantAsset]
END
GO

CREATE OR ALTER PROCEDURE [dbo].[AprimoGetAsset]
	@Id int = NULL,
	@RenditionId varchar(50) = NULL,
	@AssetId uniqueidentifier = NULL
AS
BEGIN

	SET NOCOUNT ON;

	SELECT
		Id, AssetId, RenditionId, Title, CDNUrl, ThumbnailUrl, MetaInformation, Extension, ModifiedDate
	FROM 
		[dbo].[tblAprimoPersistantAsset]
	WHERE
		(Id = @Id OR @Id IS NULL)
		AND
		(RenditionId = @RenditionId OR @RenditionId IS NULL)
		AND
		(AssetId = @AssetId OR @AssetId IS NULL)
END
GO

CREATE OR ALTER PROCEDURE [dbo].[AprimoInsertAsset]
	@Id int,
	@AssetId  uniqueidentifier,
	@RenditionId  varchar(50),
	@Title varchar(max),
	@CDNUrl varchar(max),
	@ThumbnailUrl varchar(max),
	@MetaInformation varchar(max),
	@Extension varchar(10),
	@ModifiedDate datetime
AS
BEGIN
	INSERT INTO [dbo].[tblAprimoPersistantAsset]
           ([Id],
           [AssetId],
           [RenditionId],
           [Title],
           [CDNUrl],
           [ThumbnailUrl],
           [MetaInformation],
           [Extension],
		   [ModifiedDate])
     VALUES
           (@Id,
           @AssetId,
           @RenditionId,
           @Title,
           @CDNUrl,
           @ThumbnailUrl,
           @MetaInformation,
           @Extension,
		   @ModifiedDate)
END
GO

CREATE OR ALTER PROCEDURE [dbo].[AprimoUpdateAsset]
	@Id int,
	@AssetId  uniqueidentifier,
	@RenditionId  varchar(50),
	@Title varchar(max),
	@CDNUrl varchar(max),
	@ThumbnailUrl varchar(max),
	@MetaInformation varchar(max),
	@Extension varchar(10),
	@ModifiedDate datetime
AS
BEGIN
	UPDATE [dbo].[tblAprimoPersistantAsset]
	SET 
		AssetId = @AssetId,
		RenditionId = @RenditionId,
		Title = @Title,
		CDNUrl = @CDNUrl,
		ThumbnailUrl = @ThumbnailUrl,
		MetaInformation = @MetaInformation,
		Extension = @Extension,
		ModifiedDate = @ModifiedDate
	 WHERE 
		Id = @Id
END
GO

CREATE OR ALTER PROCEDURE [dbo].[AprimoGetAssetsById]
	@AssetId UniqueIdentifier 	
AS
BEGIN
	SET NOCOUNT ON;

    SELECT
		Id, AssetId, RenditionId, Title, CDNUrl, ThumbnailUrl, MetaInformation, Extension, ModifiedDate
	FROM 
		[dbo].[tblAprimoPersistantAsset]
	WHERE 
		AssetId = @AssetId		
END
GO

CREATE OR ALTER PROCEDURE [dbo].[AprimoSearchAssets] 
	@searchTerm varchar(max)
AS
BEGIN
	SELECT
		Id, AssetId, RenditionId, Title, CDNUrl, ThumbnailUrl, MetaInformation, Extension, ModifiedDate
	FROM 
		[dbo].[tblAprimoPersistantAsset]
	WHERE 
		AssetId LIKE '%' + @searchTerm + '%' OR
		RenditionId LIKE '%' + @searchTerm + '%' OR  
		Title LIKE '%' + @searchTerm + '%' OR 
		CDNUrl LIKE '%' + @searchTerm + '%' OR 
		ThumbnailUrl LIKE '%' + @searchTerm + '%' OR 
		MetaInformation LIKE '%' + @searchTerm + '%' OR 
		Extension LIKE '%' + @searchTerm + '%' 
END
GO