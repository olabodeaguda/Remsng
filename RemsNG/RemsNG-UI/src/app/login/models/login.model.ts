export class LoginModel {
    constructor() {
    }
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
