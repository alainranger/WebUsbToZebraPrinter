using Microsoft.EntityFrameworkCore;
using ZebraLabels.Api.Domain;

namespace ZebraLabels.Api.Infrastructure.Persistence;

public sealed class ZebraLabelsDbContext(DbContextOptions<ZebraLabelsDbContext> options) : DbContext(options)
{
    public DbSet<LabelTemplate> Templates => Set<LabelTemplate>();

    public DbSet<PrinterProfile> PrinterProfiles => Set<PrinterProfile>();

    public DbSet<PrintJob> PrintJobs => Set<PrintJob>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LabelTemplate>(entity =>
        {
            entity.ToTable("templates");
            entity.HasKey(template => template.Id);
            entity.Property(template => template.Name).HasMaxLength(200).IsRequired();
            entity.Property(template => template.Description).HasMaxLength(2000);
            entity.Property(template => template.SourceLanguage).HasConversion<string>().IsRequired();
            entity.Property(template => template.RawContent).HasColumnType("text").IsRequired();
            entity.Property(template => template.Version).IsRequired();
            entity.Property(template => template.IsArchived).IsRequired();
            entity.Property(template => template.CreatedAtUtc).IsRequired();
            entity.Property(template => template.UpdatedAtUtc).IsRequired();
            entity.OwnsOne(template => template.Dimensions, dimensions =>
            {
                dimensions.Property(value => value.WidthMm).HasColumnName("width_mm");
                dimensions.Property(value => value.HeightMm).HasColumnName("height_mm");
                dimensions.Property(value => value.Dpmm).HasColumnName("dpmm");
            });
            entity.OwnsMany(template => template.Variables, variables =>
            {
                variables.ToTable("template_variables");
                variables.WithOwner().HasForeignKey("TemplateId");
                variables.Property<Guid>("Id");
                variables.HasKey("Id");
                variables.Property(variable => variable.Name).HasMaxLength(100).IsRequired();
                variables.Property(variable => variable.DisplayName).HasMaxLength(200);
                variables.Property(variable => variable.DefaultValue).HasMaxLength(500);
                variables.Property(variable => variable.ExampleValue).HasMaxLength(500);
                variables.Property(variable => variable.Order).IsRequired();
            });
        });

        modelBuilder.Entity<PrinterProfile>(entity =>
        {
            entity.ToTable("printer_profiles");
            entity.HasKey(profile => profile.Id);
            entity.Property(profile => profile.Name).HasMaxLength(200).IsRequired();
            entity.Property(profile => profile.PreferredLanguage).HasConversion<string>().IsRequired();
            entity.Property(profile => profile.VendorId);
            entity.Property(profile => profile.ProductId);
            entity.Property(profile => profile.Notes).HasMaxLength(2000);
            entity.OwnsOne(profile => profile.Capabilities, capabilities =>
            {
                capabilities.Property(value => value.SupportsEpl).HasColumnName("supports_epl");
                capabilities.Property(value => value.SupportsZpl).HasColumnName("supports_zpl");
                capabilities.Property(value => value.AllowEplToZplFallback).HasColumnName("allow_epl_to_zpl_fallback");
            });
            entity.OwnsOne(profile => profile.DefaultDimensions, dimensions =>
            {
                dimensions.Property(value => value.WidthMm).HasColumnName("default_width_mm");
                dimensions.Property(value => value.HeightMm).HasColumnName("default_height_mm");
                dimensions.Property(value => value.Dpmm).HasColumnName("default_dpmm");
            });
        });

        modelBuilder.Entity<PrintJob>(entity =>
        {
            entity.ToTable("print_jobs");
            entity.HasKey(job => job.Id);
            entity.Property(job => job.RequestedLanguage).HasConversion<string>().IsRequired();
            entity.Property(job => job.EffectiveLanguage).HasConversion<string>().IsRequired();
            entity.Property(job => job.Status).HasConversion<string>().IsRequired();
            entity.Property(job => job.Checksum).HasMaxLength(128).IsRequired();
            entity.Property(job => job.FailureReason).HasMaxLength(2000);
            entity.Property(job => job.SubmittedAtUtc).IsRequired();
        });
    }
}
