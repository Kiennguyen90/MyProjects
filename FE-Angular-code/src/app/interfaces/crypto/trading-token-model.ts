import { BaseResponseModel } from "../base-respone-model";

// Represents a request model for trading tokens in the crypto application
export interface TransactionTokenRequestModel {
    userId: string;
    tokenId: string;
    tokenName: string;
    tokenAmount: number;
    amountVnd: number;
    amountUsdt: number;
    isBuy: boolean;
}

export interface TransactionTokenModel {
    tokenAmount: number;
    price: number;
    amountVnd: number;
    amountUsdt: number;
    isBuy: boolean;
    createdBy: string;
    exchangeDate: Date;
}

export interface TokenModel {
    tokenId: string;
    tokenName: string;
    symbol: string;
    totalTokenAmount: number;
    totalValue: number;
    currentPrice: number;
    logoUrl: string;
    transactions: TransactionTokenModel[];
}

export interface UserTokensModel extends BaseResponseModel {
    tokens: TokenModel[];
}