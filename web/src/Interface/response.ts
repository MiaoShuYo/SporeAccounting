export interface Response<T> {
    statusCode: number;
    errorMessage: string;
    data: T;
}

export interface PageResponse<T> {
    pageCount: number;
    rowCount: number;
    data: T[];
}