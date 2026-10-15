using System.Globalization;
using Oracle.ManagedDataAccess.Client;
using PassScheduleApp.Models;

namespace PassScheduleApp.Services;

public sealed class OracleServerPassScheduleRepository : IPassScheduleRepository
{
    private readonly string _connectionString;

    public OracleServerPassScheduleRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
    }

    public async Task<List<PassScheduleCoilOption>> GetCoilOptionsAsync(string? coilId, DateTime? selectedDate)
    {
        const string sql = @"
            SELECT
                PDI_ID_INPUT_COIL AS ""CoilId"",
                PDI_TDC_NO AS ""TDC"",
                PDI_ID_MESSAGE,
                TOC AS ""CreationDate"",
                PDI_INP_SEC2_COIL AS ""Width"",
                PDI_INP_SEC1_COIL AS ""InputThickness"",
                PDI_INP_MS_COIL AS ""Weight"",
                NO_OF_ROLLING AS ""NoOfRolling"",
                CASE NO_OF_ROLLING
                    WHEN 0 THEN PDI_ROLLING_THICK1
                    WHEN 1 THEN PDI_ROLLING_THICK1
                    WHEN 2 THEN PDI_ROLLING_THICK2
                    WHEN 3 THEN PDI_ROLLING_THICK3
                    WHEN 4 THEN PDI_ROLLING_THICK4
                    WHEN 5 THEN PDI_ROLLING_THICK5
                    WHEN 6 THEN PDI_ROLLING_THICK6
                    WHEN 7 THEN PDI_ROLLING_THICK7
                END AS ""OutputThickness""
            FROM MILL4.T_MILL_PDI_S
            WHERE (:CoilId IS NULL OR PDI_ID_INPUT_COIL LIKE '%' || :CoilId || '%')
              AND (:SelectedDate IS NULL OR TRUNC(TOC) = TRUNC(:SelectedDate))
            ORDER BY PDI_ID_INPUT_COIL";

        var result = new List<PassScheduleCoilOption>();

        await using var connection = new OracleConnection(_connectionString);
        await using var command = new OracleCommand(sql, connection) { BindByName = true };
        AddNullableString(command, "CoilId", string.IsNullOrWhiteSpace(coilId) ? null : coilId.Trim(), 30);
        AddNullableDate(command, "SelectedDate", selectedDate);

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new PassScheduleCoilOption
            {
                CoilId = reader["CoilId"]?.ToString() ?? string.Empty,
                Tdc = reader["TDC"]?.ToString() ?? string.Empty,
                PdiIdMessage = reader["PDI_ID_MESSAGE"]?.ToString() ?? string.Empty,
                CreationDate = GetNullableDateTime(reader["CreationDate"]),
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
        const string sql = @"
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
            WHERE COIL_ID = :CoilId
            ORDER BY PASS_NO";

        var result = new List<PassScheduleOutputRow>();

        await using var connection = new OracleConnection(_connectionString);
        await using var command = new OracleCommand(sql, connection) { BindByName = true };
        AddString(command, "CoilId", coilId ?? string.Empty, 30);

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
                CreatedAt = GetNullableDateTime(reader["CREATED_AT"])
            });
        }

        return result;
    }

    public async Task<List<PassScheduleOutputEditRow>> GetEditRowsAsync(string coilId)
    {
        const string sql = @"
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
            WHERE COIL_ID = :CoilId
            ORDER BY PASS_NO";

        var result = new List<PassScheduleOutputEditRow>();

        await using var connection = new OracleConnection(_connectionString);
        await using var command = new OracleCommand(sql, connection) { BindByName = true };
        AddString(command, "CoilId", coilId ?? string.Empty, 30);

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
                Remark = reader["REMARK"]?.ToString() ?? string.Empty,
                CreatedAt = GetNullableDateTime(reader["CREATED_AT"]),
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

        await using var connection = new OracleConnection(_connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();
        var affectedRows = 0;

        try
        {
            const string deleteSql = @"
                DELETE FROM MILL4.T_PASS_SCHEDULE_OUTPUT_EDIT
                WHERE COIL_ID = :CoilId
                  AND RUN_ID = :RunId
                  AND PASS_NO = :OriginalPassNo";

            foreach (var deletedRow in deletedRowList)
            {
                await using var deleteCommand = new OracleCommand(deleteSql, connection)
                {
                    BindByName = true,
                    Transaction = transaction
                };
                AddString(deleteCommand, "CoilId", coilId, 30);
                AddString(deleteCommand, "RunId", deletedRow.RunId, 50);
                AddNullableInt(deleteCommand, "OriginalPassNo", deletedRow.OriginalPassNo);
                affectedRows += await deleteCommand.ExecuteNonQueryAsync();
            }

            foreach (var row in rowList)
            {
                var isNew = row.IsNew;
                if (isNew)
                {
                    row.RunId = $"RUN-{Guid.NewGuid():N}";
                }

                var sql = isNew ? InsertSql : UpdateSql;
                await using var command = new OracleCommand(sql, connection)
                {
                    BindByName = true,
                    Transaction = transaction
                };
                AddRowParameters(command, row, overallRemark, rowActionPerformed);
                if (!isNew)
                {
                    AddNullableInt(command, "OriginalPassNo", row.OriginalPassNo);
                }

                var changed = await command.ExecuteNonQueryAsync();
                if (!isNew && changed == 0)
                {
                    throw new InvalidOperationException($"Pass {row.OriginalPassNo} was not found for coil {coilId}; reload and try again.");
                }
                affectedRows += changed;
            }

            transaction.Commit();
            return affectedRows;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private const string InsertSql = @"
        INSERT INTO MILL4.T_PASS_SCHEDULE_OUTPUT_EDIT
            (RUN_ID, REQUEST_ID, COIL_ID, TDC, PASS_NO, ENTRY_MM, EXIT_MM, DRAFT_MM,
             REDUCTION_PCT, BACK_TENSION_KG, FRONT_TENSION_KG, SPEED_MPM, ACH_SPEED_MPM,
             FORCE_T, POWER_KW, GAP_MM, GAUGE_RISK, ML_CONF, REMARK, OVERALL_REMARK, CREATED_AT)
        VALUES
            (:RunId, :RequestId, :CoilId, :Tdc, :PassNo, :EntryMm, :ExitMm, :DraftMm,
             :ReductionPct, :BackTensionKg, :FrontTensionKg, :SpeedMpm, :AchSpeedMpm,
             :ForceT, :PowerKw, :GapMm, :GaugeRisk, :MlConf, :Remark, :OverallRemark,
             COALESCE(:CreatedAt, SYS_EXTRACT_UTC(SYSTIMESTAMP)))";

    private const string UpdateSql = @"
        UPDATE MILL4.T_PASS_SCHEDULE_OUTPUT_EDIT
        SET PASS_NO = :PassNo,
            ENTRY_MM = :EntryMm,
            EXIT_MM = :ExitMm,
            DRAFT_MM = :DraftMm,
            REDUCTION_PCT = :ReductionPct,
            BACK_TENSION_KG = :BackTensionKg,
            FRONT_TENSION_KG = :FrontTensionKg,
            SPEED_MPM = :SpeedMpm,
            ACH_SPEED_MPM = :AchSpeedMpm,
            FORCE_T = :ForceT,
            POWER_KW = :PowerKw,
            GAP_MM = :GapMm,
            GAUGE_RISK = :GaugeRisk,
            ML_CONF = :MlConf,
            REMARK = :Remark,
            OVERALL_REMARK = CASE WHEN :RowActionPerformed = 1 THEN :OverallRemark ELSE OVERALL_REMARK END
        WHERE RUN_ID = :RunId
          AND COIL_ID = :CoilId
          AND PASS_NO = :OriginalPassNo";

    private static void AddRowParameters(OracleCommand command, PassScheduleOutputEditRow row, string overallRemark, bool rowActionPerformed)
    {
        AddString(command, "RunId", row.RunId, 50);
        AddNullableInt(command, "RequestId", row.RequestId);
        AddString(command, "CoilId", row.CoilId, 30);
        AddString(command, "Tdc", row.Tdc, 10);
        AddNullableInt(command, "PassNo", row.PassNo);
        AddNullableDecimal(command, "EntryMm", row.EntryMm);
        AddNullableDecimal(command, "ExitMm", row.ExitMm);
        AddNullableDecimal(command, "DraftMm", row.DraftMm);
        AddNullableDecimal(command, "ReductionPct", row.ReductionPct);
        AddNullableDecimal(command, "BackTensionKg", row.BackTensionKg);
        AddNullableDecimal(command, "FrontTensionKg", row.FrontTensionKg);
        AddNullableDecimal(command, "SpeedMpm", row.SpeedMpm);
        AddNullableDecimal(command, "AchSpeedMpm", row.AchSpeedMpm);
        AddNullableDecimal(command, "ForceT", row.ForceT);
        AddNullableDecimal(command, "PowerKw", row.PowerKw);
        AddNullableDecimal(command, "GapMm", row.GapMm);
        AddNullableString(command, "GaugeRisk", row.GaugeRisk, 20);
        AddNullableDecimal(command, "MlConf", row.MlConf);
        AddString(command, "Remark", row.Remark ?? string.Empty, 500);
        AddNullableString(command, "OverallRemark", string.IsNullOrWhiteSpace(overallRemark) ? null : overallRemark.Trim(), 500);
        command.Parameters.Add("RowActionPerformed", OracleDbType.Int32).Value = rowActionPerformed ? 1 : 0;
        AddNullableDateTime(command, "CreatedAt", row.CreatedAt);
    }

    private static void AddString(OracleCommand command, string name, string value, int size)
    {
        command.Parameters.Add(name, OracleDbType.Varchar2, size).Value = value;
    }

    private static void AddNullableString(OracleCommand command, string name, string? value, int size)
    {
        command.Parameters.Add(name, OracleDbType.Varchar2, size).Value = string.IsNullOrWhiteSpace(value) ? DBNull.Value : value;
    }

    private static void AddNullableInt(OracleCommand command, string name, int? value)
    {
        command.Parameters.Add(name, OracleDbType.Int32).Value = value.HasValue ? value.Value : DBNull.Value;
    }

    private static void AddNullableDecimal(OracleCommand command, string name, decimal? value)
    {
        command.Parameters.Add(name, OracleDbType.Decimal).Value = value.HasValue ? value.Value : DBNull.Value;
    }

    private static void AddNullableDate(OracleCommand command, string name, DateTime? value)
    {
        command.Parameters.Add(name, OracleDbType.Date).Value = value.HasValue ? value.Value.Date : DBNull.Value;
    }

    private static void AddNullableDateTime(OracleCommand command, string name, DateTime? value)
    {
        command.Parameters.Add(name, OracleDbType.TimeStamp).Value = value.HasValue ? value.Value : DBNull.Value;
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

    private static DateTime? GetNullableDateTime(object? value)
    {
        if (value is null || value == DBNull.Value)
        {
            return null;
        }

        return Convert.ToDateTime(value, CultureInfo.InvariantCulture);
    }
}
