export const isSuperUser = () => {
    const token = localStorage.getItem('jwt');

    var jwtDecoder = require('jwt-decode');
    const decodedToken = jwtDecoder(token);
    var role = decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    console.log(role)

    if(!token || role != 'admin'){
        if(role != 'superUser'){
        return false;
        }
    }

    return decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ? true : false;
    
}