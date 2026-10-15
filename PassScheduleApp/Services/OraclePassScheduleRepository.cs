using PassScheduleApp.Models;

namespace PassScheduleApp.Services;

public class DummyPassScheduleRepository : IPassScheduleRepository
{
    private static readonly List<PassScheduleCoilOption> CoilCatalog = new()
    {
        new PassScheduleCoilOption { CoilId = "COIL-1001", Tdc = "TDC1", PdiIdMessage = "PDI-001", CreationDate = new DateTime(2026, 10, 1) },
        new PassScheduleCoilOption { CoilId = "COIL-1002", Tdc = "TDC2", PdiIdMessage = "PDI-002", CreationDate = new DateTime(2026, 10, 2) },
        new PassScheduleCoilOption { CoilId = "COIL-1003", Tdc = "TDC3", PdiIdMessage = "PDI-003", CreationDate = new DateTime(2026, 10, 3) },
        new PassScheduleCoilOption { CoilId = "COIL-2001", Tdc = "TDC4", PdiIdMessage = "PDI-004", CreationDate = new DateTime(2026, 10, 15) }
    };

    private static readonly List<PassScheduleOutputRow> OutputRows = new()
    {
        new PassScheduleOutputRow { RunId = "RUN-1001", RequestId = 1, CoilId = "COIL-1001", Tdc = "TDC1", PassNo = 1, EntryMm = 2.95m, ExitMm = 2.40m, DraftMm = 0.55m, ReductionPct = 18.25m, BackTensionKg = 120.50m, FrontTensionKg = 110.25m, SpeedMpm = 420.00m, AchSpeedMpm = 410.00m, ForceT = 88.20m, PowerKw = 204.50m, GapMm = 1.25m, GaugeRisk = "LOW", MlConf = 95.20m, CreatedAt = DateTime.Now },
        new PassScheduleOutputRow { RunId = "RUN-1002", RequestId = 2, CoilId = "COIL-1001", Tdc = "TDC1", PassNo = 2, EntryMm = 2.40m, ExitMm = 1.90m, DraftMm = 0.50m, ReductionPct = 20.80m, BackTensionKg = 130.00m, FrontTensionKg = 118.75m, SpeedMpm = 430.00m, AchSpeedMpm = 418.00m, ForceT = 92.10m, PowerKw = 210.00m, GapMm = 1.30m, GaugeRisk = "MED", MlConf = 94.50m, CreatedAt = DateTime.Now },
        new PassScheduleOutputRow { RunId = "RUN-2001", RequestId = 3, CoilId = "COIL-1002", Tdc = "TDC2", PassNo = 1, EntryMm = 3.10m, ExitMm = 2.55m, DraftMm = 0.55m, ReductionPct = 17.70m, BackTensionKg = 140.00m, FrontTensionKg = 128.00m, SpeedMpm = 410.00m, AchSpeedMpm = 402.00m, ForceT = 86.40m, PowerKw = 200.00m, GapMm = 1.35m, GaugeRisk = "LOW", MlConf = 96.10m, CreatedAt = DateTime.Now },
        new PassScheduleOutputRow { RunId = "RUN-2002", RequestId = 4, CoilId = "COIL-1002", Tdc = "TDC2", PassNo = 2, EntryMm = 2.55m, ExitMm = 2.05m, DraftMm = 0.50m, ReductionPct = 19.60m, BackTensionKg = 145.00m, FrontTensionKg = 132.00m, SpeedMpm = 415.00m, AchSpeedMpm = 408.00m, ForceT = 90.00m, PowerKw = 208.40m, GapMm = 1.40m, GaugeRisk = "LOW", MlConf = 95.80m, CreatedAt = DateTime.Now },
        new PassScheduleOutputRow { RunId = "RUN-3001", RequestId = 5, CoilId = "COIL-1003", Tdc = "TDC3", PassNo = 1, EntryMm = 2.80m, ExitMm = 2.35m, DraftMm = 0.45m, ReductionPct = 16.00m, BackTensionKg = 116.00m, FrontTensionKg = 104.00m, SpeedMpm = 405.00m, AchSpeedMpm = 399.00m, ForceT = 84.50m, PowerKw = 195.20m, GapMm = 1.20m, GaugeRisk = "LOW", MlConf = 93.80m, CreatedAt = DateTime.Now },
        new PassScheduleOutputRow { RunId = "RUN-4001", RequestId = 6, CoilId = "COIL-2001", Tdc = "TDC4", PassNo = 1, EntryMm = 3.20m, ExitMm = 2.70m, DraftMm = 0.50m, ReductionPct = 15.60m, BackTensionKg = 150.00m, FrontTensionKg = 138.00m, SpeedMpm = 435.00m, AchSpeedMpm = 425.00m, ForceT = 93.50m, PowerKw = 214.60m, GapMm = 1.45m, GaugeRisk = "MED", MlConf = 94.10m, CreatedAt = DateTime.Now }
    };

    private static readonly List<PassScheduleOutputEditRow> EditRows = new()
    {
        new PassScheduleOutputEditRow { RunId = "RUN-1001", RequestId = 1, CoilId = "COIL-1001", Tdc = "TDC1", PassNo = 1, EntryMm = 2.95m, ExitMm = 2.40m, DraftMm = 0.55m, ReductionPct = 18.25m, BackTensionKg = 120.50m, FrontTensionKg = 110.25m, SpeedMpm = 420.00m, AchSpeedMpm = 410.00m, ForceT = 88.20m, PowerKw = 204.50m, GapMm = 1.25m, GaugeRisk = "LOW", MlConf = 95.20m, CreatedAt = DateTime.Now, Remark = "Initial review" },
        new PassScheduleOutputEditRow { RunId = "RUN-1002", RequestId = 2, CoilId = "COIL-1001", Tdc = "TDC1", PassNo = 2, EntryMm = 2.40m, ExitMm = 1.90m, DraftMm = 0.50m, ReductionPct = 20.80m, BackTensionKg = 130.00m, FrontTensionKg = 118.75m, SpeedMpm = 430.00m, AchSpeedMpm = 418.00m, ForceT = 92.10m, PowerKw = 210.00m, GapMm = 1.30m, GaugeRisk = "MED", MlConf = 94.50m, CreatedAt = DateTime.Now, Remark = "Needs verification" },
        new PassScheduleOutputEditRow { RunId = "RUN-2001", RequestId = 3, CoilId = "COIL-1002", Tdc = "TDC2", PassNo = 1, EntryMm = 3.10m, ExitMm = 2.55m, DraftMm = 0.55m, ReductionPct = 17.70m, BackTensionKg = 140.00m, FrontTensionKg = 128.00m, SpeedMpm = 410.00m, AchSpeedMpm = 402.00m, ForceT = 86.40m, PowerKw = 200.00m, GapMm = 1.35m, GaugeRisk = "LOW", MlConf = 96.10m, CreatedAt = DateTime.Now, Remark = "Approved" },
        new PassScheduleOutputEditRow { RunId = "RUN-2002", RequestId = 4, CoilId = "COIL-1002", Tdc = "TDC2", PassNo = 2, EntryMm = 2.55m, ExitMm = 2.05m, DraftMm = 0.50m, ReductionPct = 19.60m, BackTensionKg = 145.00m, FrontTensionKg = 132.00m, SpeedMpm = 415.00m, AchSpeedMpm = 408.00m, ForceT = 90.00m, PowerKw = 208.40m, GapMm = 1.40m, GaugeRisk = "LOW", MlConf = 95.80m, CreatedAt = DateTime.Now, Remark = "Ready for issue" },
        new PassScheduleOutputEditRow { RunId = "RUN-3001", RequestId = 5, CoilId = "COIL-1003", Tdc = "TDC3", PassNo = 1, EntryMm = 2.80m, ExitMm = 2.35m, DraftMm = 0.45m, ReductionPct = 16.00m, BackTensionKg = 116.00m, FrontTensionKg = 104.00m, SpeedMpm = 405.00m, AchSpeedMpm = 399.00m, ForceT = 84.50m, PowerKw = 195.20m, GapMm = 1.20m, GaugeRisk = "LOW", MlConf = 93.80m, CreatedAt = DateTime.Now, Remark = "Pending QA" },
        new PassScheduleOutputEditRow { RunId = "RUN-4001", RequestId = 6, CoilId = "COIL-2001", Tdc = "TDC4", PassNo = 1, EntryMm = 3.20m, ExitMm = 2.70m, DraftMm = 0.50m, ReductionPct = 15.60m, BackTensionKg = 150.00m, FrontTensionKg = 138.00m, SpeedMpm = 435.00m, AchSpeedMpm = 425.00m, ForceT = 93.50m, PowerKw = 214.60m, GapMm = 1.45m, GaugeRisk = "MED", MlConf = 94.10m, CreatedAt = DateTime.Now, Remark = "Sample update" }
    };

    public Task<List<PassScheduleCoilOption>> GetCoilOptionsAsync(string? coilId, DateTime? selectedDate)
    {
        var filtered = CoilCatalog
            .Where(c => string.IsNullOrWhiteSpace(coilId) || c.CoilId.Contains(coilId, StringComparison.OrdinalIgnoreCase))
            .Where(c => !selectedDate.HasValue || c.CreationDate.HasValue && c.CreationDate.Value.Date == selectedDate.Value.Date)
            .OrderBy(c => c.CoilId)
            .ToList();

        return Task.FromResult(filtered);
    }

    public Task<List<PassScheduleOutputRow>> GetOutputRowsAsync(string coilId)
    {
        var rows = OutputRows
            .Where(r => r.CoilId.Equals(coilId, StringComparison.OrdinalIgnoreCase))
            .OrderBy(r => r.PassNo)
            .ToList();

        return Task.FromResult(rows);
    }

    public Task<List<PassScheduleOutputEditRow>> GetEditRowsAsync(string coilId)
    {
        var rows = EditRows
            .Where(r => r.CoilId.Equals(coilId, StringComparison.OrdinalIgnoreCase))
            .OrderBy(r => r.PassNo)
            .Select(r => new PassScheduleOutputEditRow
            {
                RunId = r.RunId,
                RequestId = r.RequestId,
                CoilId = r.CoilId,
                Tdc = r.Tdc,
                PassNo = r.PassNo,
                EntryMm = r.EntryMm,
                ExitMm = r.ExitMm,
                DraftMm = r.DraftMm,
                ReductionPct = r.ReductionPct,
                BackTensionKg = r.BackTensionKg,
                FrontTensionKg = r.FrontTensionKg,
                SpeedMpm = r.SpeedMpm,
                AchSpeedMpm = r.AchSpeedMpm,
                ForceT = r.ForceT,
                PowerKw = r.PowerKw,
                GapMm = r.GapMm,
                GaugeRisk = r.GaugeRisk,
                MlConf = r.MlConf,
                CreatedAt = r.CreatedAt,
                Remark = r.Remark,
                OriginalPassNo = r.PassNo
            })
            .ToList();

        return Task.FromResult(rows);
    }

    public Task<int> SaveEditedRowsAsync(
        string coilId,
        IEnumerable<PassScheduleOutputEditRow> rows,
        IEnumerable<DeletedPassScheduleRow> deletedRows,
        string overallRemark,
        bool rowActionPerformed)
    {
        if (rowActionPerformed && string.IsNullOrWhiteSpace(overallRemark))
        {
            throw new InvalidOperationException("Overall remark is required after adding or deleting a row.");
        }

        foreach (var deletedRow in deletedRows)
        {
            EditRows.RemoveAll(row => row.CoilId == coilId && row.RunId == deletedRow.RunId && row.PassNo == deletedRow.OriginalPassNo);
        }

        foreach (var row in rows)
        {
            if (row.IsNew)
            {
                row.RunId = $"RUN-{Guid.NewGuid():N}";
                EditRows.Add(row);
                continue;
            }

            var existing = EditRows.FirstOrDefault(r => r.CoilId == coilId && r.RunId == row.RunId && r.PassNo == row.OriginalPassNo);
            if (existing != null)
            {
                existing.PassNo = row.PassNo;
                existing.EntryMm = row.EntryMm;
                existing.ExitMm = row.ExitMm;
                existing.DraftMm = row.DraftMm;
                existing.ReductionPct = row.ReductionPct;
                existing.BackTensionKg = row.BackTensionKg;
                existing.FrontTensionKg = row.FrontTensionKg;
                existing.SpeedMpm = row.SpeedMpm;
                existing.AchSpeedMpm = row.AchSpeedMpm;
                existing.ForceT = row.ForceT;
                existing.PowerKw = row.PowerKw;
                existing.GapMm = row.GapMm;
                existing.GaugeRisk = row.GaugeRisk;
                existing.MlConf = row.MlConf;
                existing.Remark = row.Remark;
            }
        }

        return Task.FromResult(rows.Count());
    }
}
