function updateNetworks<T>(hook: React.Dispatch<React.SetStateAction<T>>, userId: String) {
    fetch(`/Networks?requestingUserID=${userId}`,{
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

export default updateNetworks;