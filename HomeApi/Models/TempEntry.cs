using System.ComponentModel.DataAnnotations;

namespace HomeApi.Models;

public class TempEntry
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public DateTime MeasueredAt { get; set; }

    [Required]
    public string Location { get; set; } = "";

    [Required]
    public double Temperature { get; set; }

    [Required]
    public double Humidity { get; set; }
}