export interface ServiceModel {
    id: string;
    name: string;
    serviceTypes: ServiceTypeModel[];
    isActive: boolean;

}

export interface ServiceTypeModel {
    id: number;
    name: string;
    description: string;
    price: number;
}