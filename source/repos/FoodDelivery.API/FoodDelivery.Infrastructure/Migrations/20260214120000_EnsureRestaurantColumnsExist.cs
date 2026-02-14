using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodDelivery.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnsureRestaurantColumnsExist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF COL_LENGTH('Restaurants', 'Email') IS NULL
                BEGIN
                    ALTER TABLE [Restaurants] ADD [Email] nvarchar(max) NOT NULL CONSTRAINT [DF_Restaurants_Email] DEFAULT N'';
                END
                """);

            migrationBuilder.Sql("""
                IF COL_LENGTH('Restaurants', 'IdentityUserId') IS NULL
                BEGIN
                    ALTER TABLE [Restaurants] ADD [IdentityUserId] nvarchar(max) NULL;
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF COL_LENGTH('Restaurants', 'IdentityUserId') IS NOT NULL
                BEGIN
                    ALTER TABLE [Restaurants] DROP COLUMN [IdentityUserId];
                END
                """);

            migrationBuilder.Sql("""
                IF COL_LENGTH('Restaurants', 'Email') IS NOT NULL
                BEGIN
                    DECLARE @ConstraintName nvarchar(200);
                    SELECT @ConstraintName = [dc].[name]
                    FROM sys.default_constraints dc
                    INNER JOIN sys.columns c ON c.default_object_id = dc.object_id
                    INNER JOIN sys.tables t ON t.object_id = c.object_id
                    WHERE t.name = 'Restaurants' AND c.name = 'Email';

                    IF @ConstraintName IS NOT NULL
                    BEGIN
                        EXEC('ALTER TABLE [Restaurants] DROP CONSTRAINT [' + @ConstraintName + ']');
                    END

                    ALTER TABLE [Restaurants] DROP COLUMN [Email];
                END
                """);
        }
    }
}
