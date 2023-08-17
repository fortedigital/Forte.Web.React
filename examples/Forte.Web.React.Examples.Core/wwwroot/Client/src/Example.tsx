import {FunctionComponent, useState} from 'react'
import './Example.css'

const Example: FunctionComponent<{ initCount: number, text: string }> = ({ initCount, text }) => {
    const [count, setCount] = useState(initCount ?? 0)

    return (
        <div className="root">
            <div className="example">
                <div className="card">
                    <button onClick={() => setCount((count) => count + 1)}>
                        count is {count}
                    </button>
                    <p>{text}</p>
                </div>
            </div>
        </div>
    )
};

export default Example
