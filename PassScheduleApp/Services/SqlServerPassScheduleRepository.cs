using System.Globalization;
using Microsoft.Data.SqlClient;
using PassScheduleApp.Models;

namespace PassScheduleApp.Services;

public class SqlServerPassScheduleRepository : IPassScheduleRepository
{
    private readonly string _connectionString;

    public SqlServerPassScheduleRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
    }

    public async Task<List<PassScheduleCoilOption>> GetCoilOptionsAsync(string? coilId, DateTime? selectedDate)
    {
        var sql = @"
            SELECT
                PDI_ID_INPUT_COIL AS CoilId,
                PDI_TDC_NO AS TDC,
                PDI_ID_MESSAGE,
                TOC AS CreationDate,
                PDI_INP_SEC2_COIL AS Width,
                PDI_INP_SEC1_COIL AS InputThickness,
                PDI_INP_MS_COIL AS Weight,
                NO_OF_ROLLING AS NoOfRolling,
                CASE NO_OF_ROLLING
                    WHEN 0 THEN PDI_ROLLING_THICK1
                    WHEN 1 THEN PDI_ROLLING_THICK1
                    WHEN 2 THEN PDI_ROLLING_THICK2
                    WHEN 3 THEN PDI_ROLLING_THICK3
                    WHEN 4 THEN PDI_ROLLING_THICK4
                    WHEN 5 THEN PDI_ROLLING_THICK5
                    WHEN 6 THEN PDI_ROLLING_THICK6
                    WHEN 7 THEN PDI_ROLLING_THICK7
                END AS OutputThickness
            FROM MILL4.T_MILL_PDI_S
            WHERE (@CoilId IS NULL OR @CoilId = '' OR PDI_ID_INPUT_COIL LIKE '%' + @CoilId + '%')
              AND (@SelectedDate IS NULL OR CAST(TOC AS date) = CAST(@SelectedDate AS date))
            ORDER BY PDI_ID_INPUT_COIL;";

        var result = new List<PassScheduleCoilOption>();

        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@CoilId", string.IsNullOrWhiteSpace(coilId) ? (object)DBNull.Value : coilId.Trim());
        command.Parameters.AddWithValue("@SelectedDate", selectedDate.HasValue ? selectedDate.Value : (object)DBNull.Value);

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new PassScheduleCoilOption
            {
                CoilId = reader["CoilId"]?.ToString() ?? string.Empty,
                Tdc = reader["TDC"]?.ToString() ?? string.Empty,
                PdiIdMessage = reader["PDI_ID_MESSAGE"]?.ToString() ?? string.Empty,
                CreationDate = reader["CreationDate"] is DateTime creationDate ? creationDate : null,
                Width = GetNullableDecimal(reader["Width"]),
                InputThickness = GetNullableDecimal(reader["InputThickness"]),
                Weight = GetNullableDecimal(reader["Weight"]),
                NoOfRolling = GetNullableInt(reader["NoOfRolling"]),
                OutputThickness = GetNullableDecimal(reader["OutputThickness"])
            });
        }

        return result;
    }

    public async Task<List<PassScheduleOutputRow>> GetOutputRowsAsync(string coilId)
    {
        var sql = @"
            SELECT
                RUN_ID,
                REQUEST_ID,
                COIL_ID,
                TDC,
                PASS_NO,
                ENTRY_MM,
                EXIT_MM,
                DRAFT_MM,
                REDUCTION_PCT,
                BACK_TENSION_KG,
                FRONT_TENSION_KG,
                SPEED_MPM,
                ACH_SPEED_MPM,
                FORCE_T,
                POWER_KW,
                GAP_MM,
                GAUGE_RISK,
                ML_CONF,
                CREATED_AT
            FROM MILL4.T_PASS_SCHEDULE_OUTPUT
            WHERE COIL_ID = @CoilId
            ORDER BY PASS_NO;";

        var result = new List<PassScheduleOutputRow>();

        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@CoilId", coilId ?? string.Empty);

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new PassScheduleOutputRow
            {
                RunId = reader["RUN_ID"]?.ToString() ?? string.Empty,
                RequestId = GetNullableInt(reader["REQUEST_ID"]),
                CoilId = reader["COIL_ID"]?.ToString() ?? string.Empty,
                Tdc = reader["TDC"]?.ToString() ?? string.Empty,
                PassNo = GetNullableInt(reader["PASS_NO"]),
                EntryMm = GetNullableDecimal(reader["ENTRY_MM"]),
                ExitMm = GetNullableDecimal(reader["EXIT_MM"]),
                DraftMm = GetNullableDecimal(reader["DRAFT_MM"]),
                ReductionPct = GetNullableDecimal(reader["REDUCTION_PCT"]),
                BackTensionKg = GetNullableDecimal(reader["BACK_TENSION_KG"]),
                FrontTensionKg = GetNullableDecimal(reader["FRONT_TENSION_KG"]),
                SpeedMpm = GetNullableDecimal(reader["SPEED_MPM"]),
                AchSpeedMpm = GetNullableDecimal(reader["ACH_SPEED_MPM"]),
                ForceT = GetNullableDecimal(reader["FORCE_T"]),
                PowerKw = GetNullableDecimal(reader["POWER_KW"]),
                GapMm = GetNullableDecimal(reader["GAP_MM"]),
                GaugeRisk = reader["GAUGE_RISK"]?.ToString(),
                MlConf = GetNullableDecimal(reader["ML_CONF"]),
                CreatedAt = reader["CREATED_AT"] is DateTime createdAt ? createdAt : null
            });
        }

        return result;
    }

    public async Task<List<PassScheduleOutputEditRow>> GetEditRowsAsync(string coilId)
    {
        var sql = @"
            SELECT
                RUN_ID,
                REQUEST_ID,
                COIL_ID,
                TDC,
                PASS_NO,
                ENTRY_MM,
                EXIT_MM,
                DRAFT_MM,
                REDUCTION_PCT,
                BACK_TENSION_KG,
                FRONT_TENSION_KG,
                SPEED_MPM,
                ACH_SPEED_MPM,
                FORCE_T,
                POWER_KW,
                GAP_MM,
                GAUGE_RISK,
                ML_CONF,
                REMARK,
                CREATED_AT
            FROM MILL4.T_PASS_SCHEDULE_OUTPUT_EDIT
            WHERE COIL_ID = @CoilId
            ORDER BY PASS_NO;";

        var result = new List<PassScheduleOutputEditRow>();

        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@CoilId", coilId ?? string.Empty);

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new PassScheduleOutputEditRow
            {
                RunId = reader["RUN_ID"]?.ToString() ?? string.Empty,
                RequestId = GetNullableInt(reader["REQUEST_ID"]),
                CoilId = reader["COIL_ID"]?.ToString() ?? string.Empty,
                Tdc = reader["TDC"]?.ToString() ?? string.Empty,
                PassNo = GetNullableInt(reader["PASS_NO"]),
                EntryMm = GetNullableDecimal(reader["ENTRY_MM"]),
                ExitMm = GetNullableDecimal(reader["EXIT_MM"]),
                DraftMm = GetNullableDecimal(reader["DRAFT_MM"]),
                ReductionPct = GetNullableDecimal(reader["REDUCTION_PCT"]),
                BackTensionKg = GetNullableDecimal(reader["BACK_TENSION_KG"]),
                FrontTensionKg = GetNullableDecimal(reader["FRONT_TENSION_KG"]),
                SpeedMpm = GetNullableDecimal(reader["SPEED_MPM"]),
                AchSpeedMpm = GetNullableDecimal(reader["ACH_SPEED_MPM"]),
                ForceT = GetNullableDecimal(reader["FORCE_T"]),
                PowerKw = GetNullableDecimal(reader["POWER_KW"]),
                GapMm = GetNullableDecimal(reader["GAP_MM"]),
                GaugeRisk = reader["GAUGE_RISK"]?.ToString(),
                MlConf = GetNullableDecimal(reader["ML_CONF"]),
                CreatedAt = reader["CREATED_AT"] is DateTime createdAt ? createdAt : null,
                Remark = reader["REMARK"]?.ToString() ?? string.Empty,
                OriginalPassNo = GetNullableInt(reader["PASS_NO"])
            });
        }

        return result;
    }

    public async Task<int> SaveEditedRowsAsync(
        string coilId,
        IEnumerable<PassScheduleOutputEditRow> rows,
        IEnumerable<DeletedPassScheduleRow> deletedRows,
        string overallRemark,
        bool rowActionPerformed)
    {
        var rowList = rows.ToList();
        var deletedRowList = deletedRows.ToList();
        if (rowActionPerformed && string.IsNullOrWhiteSpace(overallRemark))
        {
            throw new InvalidOperationException("Overall remark is required after adding or deleting a row.");
        }

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();
        var updatedCount = 0;

        try
        {
            foreach (var deletedRow in deletedRowList)
            {
                const string deleteSql = @"
                    DELETE FROM MILL4.T_PASS_SCHEDULE_OUTPUT_EDIT
                    WHERE COIL_ID = @CoilId
                      AND RUN_ID = @RunId
                      AND PASS_NO = @OriginalPassNo;";

                await using var deleteCommand = new SqlCommand(deleteSql, connection, transaction);
                deleteCommand.Parameters.AddWithValue("@CoilId", coilId);
                deleteCommand.Parameters.AddWithValue("@RunId", deletedRow.RunId);
                deleteCommand.Parameters.AddWithValue("@OriginalPassNo", GetParameterValue(deletedRow.OriginalPassNo));
                updatedCount += await deleteCommand.ExecuteNonQueryAsync();
            }

            foreach (var row in rowList)
            {
                var sql = row.IsNew
                    ? @"
                        INSERT INTO MILL4.T_PASS_SCHEDULE_OUTPUT_EDIT
                            (RUN_ID, REQUEST_ID, COIL_ID, TDC, PASS_NO, ENTRY_MM, EXIT_MM, DRAFT_MM,
                             REDUCTION_PCT, BACK_TENSION_KG, FRONT_TENSION_KG, SPEED_MPM, ACH_SPEED_MPM,
                             FORCE_T, POWER_KW, GAP_MM, GAUGE_RISK, ML_CONF, REMARK, OVERALL_REMARK, CREATED_AT)
                        VALUES
                            (@RunId, @RequestId, @CoilId, @Tdc, @PassNo, @EntryMm, @ExitMm, @DraftMm,
                             @ReductionPct, @BackTensionKg, @FrontTensionKg, @SpeedMpm, @AchSpeedMpm,
                             @ForceT, @PowerKw, @GapMm, @GaugeRisk, @MlConf, @Remark, @OverallRemark,
                             COALESCE(@CreatedAt, SYSUTCDATETIME()));"
                    : @"
                        UPDATE MILL4.T_PASS_SCHEDULE_OUTPUT_EDIT
                        SET PASS_NO = @PassNo,
                            ENTRY_MM = @EntryMm,
                            EXIT_MM = @ExitMm,
                            DRAFT_MM = @DraftMm,
                            REDUCTION_PCT = @ReductionPct,
                            BACK_TENSION_KG = @BackTensionKg,
                            FRONT_TENSION_KG = @FrontTensionKg,
                            SPEED_MPM = @SpeedMpm,
                            ACH_SPEED_MPM = @AchSpeedMpm,
                            FORCE_T = @ForceT,
                            POWER_KW = @PowerKw,
                            GAP_MM = @GapMm,
                            GAUGE_RISK = @GaugeRisk,
                            ML_CONF = @MlConf,
                            REMARK = @Remark,
                            OVERALL_REMARK = CASE WHEN @RowActionPerformed = 1 THEN @OverallRemark ELSE OVERALL_REMARK END
                        WHERE RUN_ID = @RunId
                          AND COIL_ID = @CoilId
                          AND PASS_NO = @OriginalPassNo;";

                if (row.IsNew)
                {
                    row.RunId = $"RUN-{Guid.NewGuid():N}";
                }

                await using var command = new SqlCommand(sql, connection, transaction);
                AddRowParameters(command, row, overallRemark, rowActionPerformed);
                if (!row.IsNew)
                {
                    command.Parameters.AddWithValue("@OriginalPassNo", GetParameterValue(row.OriginalPassNo));
                }

                var affected = await command.ExecuteNonQueryAsync();
                if (!row.IsNew && affected == 0)
                {
                    throw new InvalidOperationException($"Pass {row.OriginalPassNo} was not found for coil {coilId}; reload and try again.");
                }
                updatedCount += affected;
            }

            await transaction.CommitAsync();
            return updatedCount;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private static void AddRowParameters(SqlCommand command, PassScheduleOutputEditRow row, string overallRemark, bool rowActionPerformed)
    {
        command.Parameters.AddWithValue("@RunId", row.RunId ?? string.Empty);
        command.Parameters.AddWithValue("@RequestId", GetParameterValue(row.RequestId));
        command.Parameters.AddWithValue("@CoilId", row.CoilId ?? string.Empty);
        command.Parameters.AddWithValue("@Tdc", row.Tdc ?? string.Empty);
        command.Parameters.AddWithValue("@PassNo", GetParameterValue(row.PassNo));
        command.Parameters.AddWithValue("@EntryMm", GetParameterValue(row.EntryMm));
        command.Parameters.AddWithValue("@ExitMm", GetParameterValue(row.ExitMm));
        command.Parameters.AddWithValue("@DraftMm", GetParameterValue(row.DraftMm));
        command.Parameters.AddWithValue("@ReductionPct", GetParameterValue(row.ReductionPct));
        command.Parameters.AddWithValue("@BackTensionKg", GetParameterValue(row.BackTensionKg));
        command.Parameters.AddWithValue("@FrontTensionKg", GetParameterValue(row.FrontTensionKg));
        command.Parameters.AddWithValue("@SpeedMpm", GetParameterValue(row.SpeedMpm));
        command.Parameters.AddWithValue("@AchSpeedMpm", GetParameterValue(row.AchSpeedMpm));
        command.Parameters.AddWithValue("@ForceT", GetParameterValue(row.ForceT));
        command.Parameters.AddWithValue("@PowerKw", GetParameterValue(row.PowerKw));
        command.Parameters.AddWithValue("@GapMm", GetParameterValue(row.GapMm));
        command.Parameters.AddWithValue("@GaugeRisk", string.IsNullOrWhiteSpace(row.GaugeRisk) ? (object)DBNull.Value : row.GaugeRisk);
        command.Parameters.AddWithValue("@MlConf", GetParameterValue(row.MlConf));
        command.Parameters.AddWithValue("@Remark", row.Remark ?? string.Empty);
        command.Parameters.AddWithValue("@OverallRemark", string.IsNullOrWhiteSpace(overallRemark) ? (object)DBNull.Value : overallRemark.Trim());
        command.Parameters.AddWithValue("@RowActionPerformed", rowActionPerformed);
        command.Parameters.AddWithValue("@CreatedAt", row.CreatedAt.HasValue ? row.CreatedAt.Value : DBNull.Value);
    }

    private static int? GetNullableInt(object? value)
    {
        if (value is null || value == DBNull.Value)
        {
            return null;
        }

        return Convert.ToInt32(value, CultureInfo.InvariantCulture);
    }

    private static decimal? GetNullableDecimal(object? value)
    {
        if (value is null || value == DBNull.Value)
        {
            return null;
        }

        return Convert.ToDecimal(value, CultureInfo.InvariantCulture);
    }

    private static object GetParameterValue<T>(T? value) where T : struct
    {
        return value.HasValue ? value.Value : DBNull.Value;
    }
}
