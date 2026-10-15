using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PassScheduleApp.Models;

public class PassScheduleFilterViewModel
{
    [Display(Name = "Coil Id")]
    public string? CoilId { get; set; }

    [Display(Name = "Date")]
    [DataType(DataType.Date)]
    public DateTime? SelectedDate { get; set; }

    [Display(Name = "Selected Coil")]
    public string? SelectedCoilId { get; set; }
    public PassScheduleCoilOption? SelectedCoilData { get; set; }

    [Display(Name = "Overall Remark")]
    public string? OverallRemark { get; set; }
    public bool RowActionPerformed { get; set; }

    public List<SelectListItem> CoilOptions { get; set; } = new();
    public List<PassScheduleOutputRow> OutputRows { get; set; } = new();
    public List<PassScheduleOutputEditRow> EditRows { get; set; } = new();
    public List<DeletedPassScheduleRow> DeletedRows { get; set; } = new();
}

public class DeletedPassScheduleRow
{
    public string RunId { get; set; } = string.Empty;
    public int? OriginalPassNo { get; set; }
}

public class PassScheduleCoilOption
{
    public string CoilId { get; set; } = string.Empty;
    public string Tdc { get; set; } = string.Empty;
    public string PdiIdMessage { get; set; } = string.Empty;
    public DateTime? CreationDate { get; set; }
    public decimal? Width { get; set; }
    public decimal? InputThickness { get; set; }
    public decimal? Weight { get; set; }
    public int? NoOfRolling { get; set; }
    public decimal? OutputThickness { get; set; }
}

public class PassScheduleOutputRow
{
    public string RunId { get; set; } = string.Empty;
    public int? RequestId { get; set; }
    public string CoilId { get; set; } = string.Empty;
    public string Tdc { get; set; } = string.Empty;
    public int? PassNo { get; set; }
    public decimal? EntryMm { get; set; }
    public decimal? ExitMm { get; set; }
    public decimal? DraftMm { get; set; }
    public decimal? ReductionPct { get; set; }
    public decimal? BackTensionKg { get; set; }
    public decimal? FrontTensionKg { get; set; }
    public decimal? SpeedMpm { get; set; }
    public decimal? AchSpeedMpm { get; set; }
    public decimal? ForceT { get; set; }
    public decimal? PowerKw { get; set; }
    public decimal? GapMm { get; set; }
    public string? GaugeRisk { get; set; }
    public decimal? MlConf { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public class PassScheduleOutputEditRow : PassScheduleOutputRow
{
    [Display(Name = "Remark")]
    [Required(ErrorMessage = "Remark is required for every pass row.")]
    public string? Remark { get; set; }
    public int? OriginalPassNo { get; set; }
    public bool IsNew { get; set; }
}
