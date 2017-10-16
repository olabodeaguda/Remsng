export class LoginModel {
    validatedUsername: string = '';
    username: string = '';
    pwd: string = '';
    isUsernameValid: boolean = false;
    domainIds: object[];
    domainId: string = '';
    isLoading: boolean = false;
    isError: boolean = false;
    errmsg: string = '';
    errorClass: string[] = [];
}
