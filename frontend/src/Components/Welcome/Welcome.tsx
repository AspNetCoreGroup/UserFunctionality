import './Welcome.css'


interface WelcomeProps {
    username: String | undefined
}

const Welcome = (props: WelcomeProps) => {
    return (
        <div className="welcome">
            Logged in as <a href=''>{props.username}</a>
        </div>
    )
}

export default Welcome;