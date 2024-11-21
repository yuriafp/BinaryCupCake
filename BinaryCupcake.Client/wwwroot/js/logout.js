window.logout = () => {
    localStorage.removeItem('access_token');
    localStorage.removeItem('tokenRenovacao');
    location.reload();
}