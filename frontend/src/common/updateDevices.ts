function updateDevices<T>(hook: React.Dispatch<React.SetStateAction<T>>, netId: number) {
    const backendServerUri = `${process.env.REACT_APP_BACKEND_SERVER_URI}`;

    fetch(`${backendServerUri}/backend/networks/${netId}/devices`,{
        method: 'GET',
        cache: 'no-cache',
        headers: {
            "Content-Type": "application/json",
        }
    })
    .then(response => response.json())
    .then((json) => {
        hook(json as T);
    })
    .catch(() => console.log('Error fetching'));
};

export default updateDevices;