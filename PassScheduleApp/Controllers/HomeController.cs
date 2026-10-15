using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PassScheduleApp.Models;
using PassScheduleApp.Services;

namespace PassScheduleApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IPassScheduleRepository _repository;

    public HomeController(ILogger<HomeController> logger, IPassScheduleRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? coilId, DateTime? selectedDate, string? selectedCoilId)
    {
        var model = new PassScheduleFilterViewModel
        {
            CoilId = coilId,
            SelectedDate = selectedDate ?? DateTime.Today,
            SelectedCoilId = selectedCoilId
        };

        var hasFilter = !string.IsNullOrWhiteSpace(coilId) || model.SelectedDate.HasValue;
        var hasSelectedCoil = !string.IsNullOrWhiteSpace(selectedCoilId);
        var coilOptions = new List<PassScheduleCoilOption>();

        if (hasFilter || hasSelectedCoil)
        {
            var lookupCoilId = string.IsNullOrWhiteSpace(coilId) ? selectedCoilId : coilId;
            coilOptions = await _repository.GetCoilOptionsAsync(lookupCoilId, model.SelectedDate);
            model.CoilOptions = coilOptions
                .Select(c => new SelectListItem
                {
                    Value = c.CoilId,
                    Text = $"{c.CoilId} | {c.Tdc} | {c.PdiIdMessage}"
                })
                .ToList();

            if (string.IsNullOrWhiteSpace(model.SelectedCoilId) && hasFilter && model.CoilOptions.Count > 0)
            {
                model.SelectedCoilId = model.CoilOptions.First().Value;
            }
        }

        if (!string.IsNullOrWhiteSpace(model.SelectedCoilId))
        {
            model.SelectedCoilData = coilOptions.FirstOrDefault(coil => coil.CoilId == model.SelectedCoilId);
            model.OutputRows = await _repository.GetOutputRowsAsync(model.SelectedCoilId);
            model.EditRows = await _repository.GetEditRowsAsync(model.SelectedCoilId);

            if (model.EditRows.Count == 0 && model.OutputRows.Count > 0)
            {
                model.EditRows = model.OutputRows
                    .Select(row => new PassScheduleOutputEditRow
                    {
                        RunId = row.RunId,
                        RequestId = row.RequestId,
                        CoilId = row.CoilId,
                        Tdc = row.Tdc,
                        PassNo = row.PassNo,
                        EntryMm = row.EntryMm,
                        ExitMm = row.ExitMm,
                        DraftMm = row.DraftMm,
                        ReductionPct = row.ReductionPct,
                        BackTensionKg = row.BackTensionKg,
                        FrontTensionKg = row.FrontTensionKg,
                        SpeedMpm = row.SpeedMpm,
                        AchSpeedMpm = row.AchSpeedMpm,
                        ForceT = row.ForceT,
                        PowerKw = row.PowerKw,
                        GapMm = row.GapMm,
                        GaugeRisk = row.GaugeRisk,
                        MlConf = row.MlConf,
                        CreatedAt = row.CreatedAt,
                        Remark = string.Empty,
                        OriginalPassNo = row.PassNo,
                        IsNew = true
                    })
                    .ToList();
            }
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveEdit(PassScheduleFilterViewModel model)
    {
        var coilOptions = await _repository.GetCoilOptionsAsync(model.CoilId, model.SelectedDate);
        model.CoilOptions = coilOptions
            .Select(c => new SelectListItem
            {
                Value = c.CoilId,
                Text = $"{c.CoilId} | {c.Tdc} | {c.PdiIdMessage}"
            })
            .ToList();
        model.SelectedCoilData = coilOptions.FirstOrDefault(coil => coil.CoilId == model.SelectedCoilId);

        var rowActionPerformed = model.RowActionPerformed
            || model.EditRows.Any(row => row.IsNew)
            || model.DeletedRows.Count > 0;
        if (rowActionPerformed && string.IsNullOrWhiteSpace(model.OverallRemark))
        {
            ModelState.AddModelError(nameof(model.OverallRemark), "Overall remark is required after adding or deleting a row.");
        }

        if (model.EditRows.Any(row => !row.PassNo.HasValue || row.PassNo <= 0))
        {
            ModelState.AddModelError(nameof(model.EditRows), "Pass No must be a positive number for every row.");
        }

        var duplicatePassNumbers = model.EditRows
            .Where(row => row.PassNo.HasValue)
            .GroupBy(row => row.PassNo)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key);
        if (duplicatePassNumbers.Any())
        {
            ModelState.AddModelError(nameof(model.EditRows), "Pass No values must be unique for the selected coil.");
        }

        if (!ModelState.IsValid)
        {
            if (!string.IsNullOrWhiteSpace(model.SelectedCoilId))
            {
                model.OutputRows = await _repository.GetOutputRowsAsync(model.SelectedCoilId);
            }

            return View("Index", model);
        }

        try
        {
            var selectedCoil = coilOptions.FirstOrDefault(option => option.CoilId == model.SelectedCoilId);
            foreach (var row in model.EditRows)
            {
                row.CoilId = model.SelectedCoilId ?? string.Empty;
                row.Tdc = selectedCoil?.Tdc ?? row.Tdc;
            }

            await _repository.SaveEditedRowsAsync(
                model.SelectedCoilId ?? string.Empty,
                model.EditRows,
                model.DeletedRows,
                model.OverallRemark ?? string.Empty,
                rowActionPerformed);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            if (!string.IsNullOrWhiteSpace(model.SelectedCoilId))
            {
                model.OutputRows = await _repository.GetOutputRowsAsync(model.SelectedCoilId);
            }

            return View("Index", model);
        }

        TempData["SuccessMessage"] = "Pass schedule edit saved successfully.";
        return RedirectToAction(nameof(Index), new { coilId = model.CoilId, selectedDate = model.SelectedDate, selectedCoilId = model.SelectedCoilId });
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
