using System.Collections.Generic;
using AgileVentures.TezPusher.Model.Interfaces;

namespace AgileVentures.TezPusher.Model.RpcEntities
{
    /// <summary>
    /// The total amount of frozen deposits/rewards/fees.
    /// </summary>
    public class FrozenBalanceByCycle
    {
        public long cycle { get; set; }
        public string deposit { get; set; }
        public string fees { get; set; }
        public string rewards { get; set; }
    }

    public class DelegateRpcEntity : IRpcEntity
    {
        /// The full balance of a delegate: i.e.the balance of its delegate account plus the total amount of frozen deposits/rewards/fees.
        /// </summary>
        public string balance { get; set; }

        /// <summary>
        /// The details of frozen deposits/rewards/fees indexed by the cycle !!!at the end of which!!! they will be unfrozen.
        /// </summary>
        public string frozen_balance { get; set; }

        /// <summary>
        /// The total amount of frozen deposits/rewards/fees by cycle
        /// </summary>
        public List<FrozenBalanceByCycle> frozen_balance_by_cycle { get; set; }

        /// <summary>
        /// The total amount of tokens the delegate stakes for. This includes the balance of all contracts delegated to <pkh>, but also the delegate own account and the frozen deposits and fees. This does not include the frozen rewards.
        /// </summary>
        public string staking_balance { get; set; }

        /// <summary>
        /// Returns the list of contracts that delegate to a given delegate.
        /// </summary>
        public List<string> delegated_contracts { get; set; }

        /// <summary>
        /// Returns the balances of all the contracts that delegate to a given delegate. This excludes the delegate's own balance and its frozen balances.
        /// </summary>
        public string delegated_balance { get; set; }

        /// <summary>
        /// Tells whether the delegate is currently tagged as deactivated or not.
        /// </summary>
        public bool deactivated { get; set; }

        /// <summary>
        /// Returns the cycle by the end of which the delegate might be deactivated if she fails to execute any delegate action. A deactivated delegate might be reactivated (without loosing any rolls) by simply re-registering as a delegate. For deactivated delegates, this value contains the cycle by which they were deactivated.
        /// </summary>
        public int grace_period { get; set; }
    }

    public class DelegateBalanceRpcEntity : IRpcEntity
    {
        public string balance { get; set; }
    }

    public class DelegatedContractsRpcEntity : IRpcEntity
    {
        public List<string> delegated_contracts { get; set; }
    }

    public class DelegateRpcEntityError
    {
        public DelegateRpcEntityErrorDetails[] Errors { get; set; }
    }

    public class DelegateRpcEntityErrorDetails
    {
        public string kind { get; set; }
        public string id { get; set; }
        public List<string> missing_key { get; set; }
        public string function { get; set; }
    }
}