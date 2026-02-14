using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodDelivery.Infrastructure.Migrations
{
    public partial class RemoveRestaurantIdentityLinkAndAddPassword : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF COL_LENGTH('Restaurants', 'PasswordHash') IS NULL
                BEGIN
                    ALTER TABLE [Restaurants] ADD [PasswordHash] nvarchar(max) NOT NULL CONSTRAINT [DF_Restaurants_PasswordHash] DEFAULT N'';
                END

                IF COL_LENGTH('Restaurants', 'IdentityUserId') IS NOT NULL
                BEGIN
                    ALTER TABLE [Restaurants] DROP COLUMN [IdentityUserId];
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF COL_LENGTH('Restaurants', 'IdentityUserId') IS NULL
                BEGIN
                    ALTER TABLE [Restaurants] ADD [IdentityUserId] nvarchar(max) NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF COL_LENGTH('Restaurants', 'PasswordHash') IS NOT NULL
                BEGIN
                    ALTER TABLE [Restaurants] DROP CONSTRAINT IF EXISTS [DF_Restaurants_PasswordHash];
                    ALTER TABLE [Restaurants] DROP COLUMN [PasswordHash];
                END
            ");
        }
    }
}
