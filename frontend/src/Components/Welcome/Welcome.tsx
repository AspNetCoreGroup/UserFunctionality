import './Welcome.css'


interface WelcomeProps {
    username: string | undefined
}

const Welcome = (props: WelcomeProps) => {
    return (
        <div className="welcome">
            Добро пожаловать, {props.username}
        </div>
    )
}

export default Welcome;