export default interface RegisterModel {
    userName: string;
    email: string;
    password: string;
    telegramm: string | null | undefined;
    isAdmin: boolean;
}