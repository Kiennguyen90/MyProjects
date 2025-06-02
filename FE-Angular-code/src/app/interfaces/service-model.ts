export interface ServiceModel {
    id: string;
    name: string;
    serviceTypes: ServiceTypeModel[];
}

export interface ServiceTypeModel {
    id: number;
    name: string;
    description: string;
    price: number;
}