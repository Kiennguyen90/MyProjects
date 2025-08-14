
import { BaseResponseModel } from "../base-respone-model";

export interface CryptoTokenModel {
    id: string;
    name: string;
    symbol: string;
    // tokenType: string; // e.g., ERC20, BEP20
    // totalSupply: number;
    // circulatingSupply: number;
    // marketCap: number;
    // price: number;
    // volume24h: number;
    // change24h: number; // Percentage change in the last 24 hours
    // contractAddress: string; // Address of the token's smart contract
    // logoUrl?: string; // Optional URL for the token's logo
}
export interface ListCryptoTokensModel extends BaseResponseModel {
    cryptoTokens: CryptoTokenModel[];
}