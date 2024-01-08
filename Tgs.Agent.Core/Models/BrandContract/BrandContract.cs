using Tgs.Agent.Core.Models.Enums;

namespace Tgs.Agent.Core.Models.BrandContract
{
    public record BrandContract(
        Guid BrandId,
        string Name,
        short ContractDividendType,
        decimal GradeIntervalLimit,
        decimal ValidBetAmount,
        decimal DepositAmount,
        int BetDays,
        ContractGameType[] GameTypes,
        IEnumerable<BrandContractCondition> Conditions);
}
