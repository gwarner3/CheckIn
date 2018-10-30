namespace CheckInWeb.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeedAdminUser : DbMigration
    {
        public override void Up()
        {
            Sql(@"
            INSERT INTO [dbo].[AspNetUsers] ([Id], [UserName], [PasswordHash], [SecurityStamp], [Discriminator]) VALUES (N'a4665fd8-6c6f-4540-b50a-7df1face06b2', N'Adminck', N'AOJT0r23EzIGUSfgSPmRYZk+bdUgh1rZoeBAPGR4NSirNao3Knsrg7lptfip1uBzjg==', N'4df6a87c-5045-45f1-bdfd-362f59abfe7d', N'ApplicationUser')

            INSERT INTO [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'85afea3f-4a42-4c15-81f3-1e868ede7bf6', N'Admin')

            INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'a4665fd8-6c6f-4540-b50a-7df1face06b2', N'85afea3f-4a42-4c15-81f3-1e868ede7bf6')

            ");
        }
        
        public override void Down()
        {
        }
    }
}
