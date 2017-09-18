
export class ResponseModel {
    code: string;
    data: object[] = [];
    description: string;
    status: boolean;
    errors: string[];
    noOfRecords: number;
    totalPages: number;
}
