declare module '@tezos-live/push' {
	import { BlockDelegationContent, BlockOriginationContent, BlockTransactionContent, Header } from '@tezos-live';

	export interface DelegationModel {
		blockHash: string;
		blockLevel: number;
		delegationContent: BlockDelegationContent;
		operationHash: string;
		timestamp: string;
	}

	export interface HeadModel extends Header {
		chain_id: string;
		hash: string;
		protocol: string;
	}

	export interface OriginationModel {
		blockHash: string;
		blockLevel: number;
		operationHash: string;
		originationContent: BlockOriginationContent;
		timestamp: string;
	}

	export interface PushMessage {
		blockHeader: HeadModel;
		delegation: DelegationModel;
		messageType: string;
		origination: OriginationModel;
		transaction: TransactionModel;
	}

	export interface SubscribeModel {
		delegationAddresses: string[];
		originationAddresses: string[];
		transactionAddresses: string[];
		userId: string;
	}

	export interface TransactionModel {
		blockHash: string;
		blockLevel: number;
		operationHash: string;
		timestamp: string;
		transactionContent: BlockTransactionContent;
	}

}

declare module '@tezos-live' {

	export interface BakingRight {
		delegate: string;
		level: number;
		priority: number;
	}

	export interface BakingRightsRpcEntity {
		rights: BakingRight[];
	}

	export interface BalanceUpdate {
		category: string;
		change: string;
		contract: string;
		cycle?: number;
		delegate: string;
		kind: string;
	}

	export interface BlockDelegationContent extends BlockOperationContent {
		delegate: string;
		metadata: BlockTransactionMetadata;
	}

	export interface BlockDelegationsRpcEntity extends BlockOperation[] {
	}

	export interface BlockDoubleBakingBlockHash {
		context: string;
		fitness: string[];
		level: number;
		operations_hash: string;
		predecessor: string;
		priority: number;
		proof_of_work_nonce: string;
		proto: number;
		signature: string;
		timestamp: string;
		validation_pass: number;
	}

	export interface BlockDoubleBakingContent {
		bh1: BlockDoubleBakingBlockHash;
		bh2: BlockDoubleBakingBlockHash;
		kind: string;
		metadata: Metadata;
	}

	export interface BlockDoubleBakingsRpcEntity extends BlockOperation[] {
	}

	export interface BlockEndorsementBalanceUpdate {
		category: string;
		change: string;
		contract: string;
		delegate: string;
		kind: string;
		level?: number;
	}

	export interface BlockEndorsementContent {
		kind: string;
		level: number;
		metadata: BlockTransactionMetadata;
	}

	export interface BlockEndorsementMetadata {
		balance_updates: BlockEndorsementBalanceUpdate[];
		delegate: string;
		slots: number[];
	}

	export interface BlockEndorsementsRpcEntity extends BlockOperation[] {
	}

	export interface BlockLevelRpcEntity {
		cycle: number;
		cycle_position: number;
		expected_commitment: boolean;
		level: number;
		level_position: number;
		voting_period: number;
		voting_period_position: number;
	}

	export interface BlockOperation<T> {
		branch: string;
		chain_id: string;
		contents: T[];
		hash: string;
		protocol: string;
		signature: string;
	}

	export interface BlockOperationContent {
		counter: string;
		fee: string;
		gas_limit: string;
		kind: string;
		source: string;
		storage_limit: string;
	}

	export interface BlockOperationsRpcEntity extends any[][] {
	}

	export interface BlockOriginationContent extends BlockOperationContent {
		balance: string;
		metadata: BlockTransactionMetadata;
	}

	export interface BlockOriginationsRpcEntity extends BlockOperation[] {
	}

	export interface BlockRpcEntity {
		chain_id: string;
		hash: string;
		header: Header;
		metadata: Metadata;
		operations: BlockOperationsRpcEntity;
		protocol: string;
	}

	export interface BlockTransactionBalanceUpdate {
		category: string;
		change: string;
		contract: string;
		cycle?: number;
		delegate: string;
		kind: string;
	}

	export interface BlockTransactionContent extends BlockOperationContent {
		amount: string;
		destination: string;
		metadata: BlockTransactionMetadata;
	}

	export interface BlockTransactionInternalOperationResult {
		amount: string;
		destination: string;
		kind: string;
		nonce: number;
		result: BlockTransactionOperationResult;
		source: string;
	}

	export interface BlockTransactionMetadata {
		balance_updates: BlockTransactionBalanceUpdate[];
		internal_operation_results: BlockTransactionInternalOperationResult[];
		operation_result: BlockTransactionOperationResult;
	}

	export interface BlockTransactionOperationResult {
		balance_updates: BlockTransactionBalanceUpdate[];
		consumed_gas: string;
		originated_contracts: string[];
		paid_storage_size_diff: string;
		status: string;
		storage_size: string;
	}

	export interface BlockTransactionsRpcEntity extends BlockOperation[] {
	}

	export interface ConstantsRpcEntity {
		block_reward: string;
		block_security_deposit: string;
		blocks_per_commitment: number;
		blocks_per_cycle: number;
		blocks_per_roll_snapshot: number;
		blocks_per_voting_period: number;
		cost_per_byte: string;
		dictator_pubkey: string;
		endorsement_reward: string;
		endorsement_security_deposit: string;
		endorsers_per_block: number;
		hard_gas_limit_per_block: string;
		hard_gas_limit_per_operation: string;
		hard_storage_limit_per_block: string;
		hard_storage_limit_per_operation: string;
		max_operation_data_length: number;
		max_revelations_per_block: number;
		michelson_maximum_type_size: number;
		nonce_length: number;
		origination_burn: string;
		preserved_cycles: number;
		proof_of_work_nonce_size: number;
		proof_of_work_threshold: string;
		seed_nonce_revelation_tip: string;
		time_between_blocks: string[];
		tokens_per_roll: string;
	}

	export interface ContractBalanceRpcEntity {
		balance: string;
	}

	export interface ContractRpcEntity {
		balance: string;
		counter: number;
		delegate: Delegate;
		manager: string;
		spendable: boolean;
	}

	export interface Delegate {
		setable: boolean;
	}

	export interface DelegateBalanceRpcEntity {
		balance: string;
	}

	export interface DelegateRpcEntity {
		balance: string;
		deactivated: boolean;
		delegated_balance: string;
		delegated_contracts: string[];
		frozen_balance: string;
		frozen_balance_by_cycle: FrozenBalanceByCycle[];
		grace_period: number;
		staking_balance: string;
	}

	export interface DelegateRpcEntityError {
		errors: DelegateRpcEntityErrorDetails[];
	}

	export interface DelegateRpcEntityErrorDetails {
		_function: string;
		id: string;
		kind: string;
		missing_key: string[];
	}

	export interface DelegatedContractsRpcEntity {
		delegated_contracts: string[];
	}

	export interface EndorsingRight {
		delegate: string;
		level: number;
		slots: number[];
	}

	export interface EndorsingRightsRpcEntity {
		rights: EndorsingRight[];
	}

	export interface ErrorRpcEntity {
		_function: string;
		id: string;
		kind: string;
		missing_key: string[];
	}

	export interface FrozenBalanceByCycle {
		cycle: number;
		deposit: string;
		fees: string;
		rewards: string;
	}

	export interface GenericBlockOperationContent extends BlockOperationContent {
		amount: string;
		balance: string;
		delegate: string;
		destination: string;
		metadata: BlockTransactionMetadata;
	}

	export interface GenericOperationsEntity extends BlockOperation[] {
	}

	export interface Header {
		context: string;
		fitness: string[];
		level: number;
		operations_hash: string;
		predecessor: string;
		priority: number;
		proof_of_work_nonce: string;
		proto: number;
		signature: string;
		timestamp: string;
		validation_pass: number;
	}

	export interface MaxOperationListLength {
		max_op: number;
		max_size: number;
	}

	export interface Metadata {
		baker: string;
		balance_updates: BalanceUpdate[];
		consumed_gas: string;
		deactivated: any[];
		level: BlockLevelRpcEntity;
		max_block_header_length: number;
		max_operation_data_length: number;
		max_operation_list_length: MaxOperationListLength[];
		max_operations_ttl: number;
		next_protocol: string;
		nonce_hash: any;
		protocol: string;
		test_chain_status: TestChainStatus;
		voting_period_kind: string;
	}

	export interface MonitorHeadModel {
		context: string;
		fitness: string[];
		hash: string;
		level: number;
		operations_hash: string;
		predecessor: string;
		proto: number;
		protocol_data: string;
		timestamp: string;
		validation_pass: number;
	}

	export interface SelectedRollSnapshotRpcEntity {
		selected_snapshot: number;
	}

	export interface TestChainStatus {
		status: string;
	}

}

