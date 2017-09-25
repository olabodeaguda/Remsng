export class ProfileModel {
    id: string = '';
    email: string = '';
    username: string = '';
    userStatus: string = '';
    surname: string = '';
    firstname: string = '';
    lastname: string = '';     
    isErrMsg: boolean = false;
    errMsg: string = '';
    eventType: string = '';
    errClass: string[] = [];
    isLoading: boolean = false;
    gender: string = '';
}