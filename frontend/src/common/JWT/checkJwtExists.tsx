const checkJwtExists = () => !!document.cookie.split(';').find(c => c.includes('token'));

export default checkJwtExists;