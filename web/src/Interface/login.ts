export interface Login {
    username: string;
    password: string;
}

export interface LoginResponse {
    token: string;
    refreshToken: string;
}

export interface Register {
    userName: string;
    password: string;
    rePassword: string;
    phoneNumber: string;
    email: string;
}
export interface RetrievePassword {
    userName: string;
    phoneNumber: string;
    email: string;
}