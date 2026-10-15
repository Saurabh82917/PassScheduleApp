using PassScheduleApp.Models;

namespace PassScheduleApp.Services;

public interface IPassScheduleRepository
{
    Task<List<PassScheduleCoilOption>> GetCoilOptionsAsync(string? coilId, DateTime? selectedDate);
    Task<List<PassScheduleOutputRow>> GetOutputRowsAsync(string coilId);
    Task<List<PassScheduleOutputEditRow>> GetEditRowsAsync(string coilId);
    Task<int> SaveEditedRowsAsync(
        string coilId,
        IEnumerable<PassScheduleOutputEditRow> rows,
        IEnumerable<DeletedPassScheduleRow> deletedRows,
        string overallRemark,
        bool rowActionPerformed);
}
