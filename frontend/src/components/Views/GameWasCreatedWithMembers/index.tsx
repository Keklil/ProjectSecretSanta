import React, {FC} from "react";

import './style.css';

const GameWasCreatedWithMembers:FC = () => {
    return(<div className='games_view'>
        <div className="button_back"></div>
        <section className="container">
            <div className="info_window">
                <p className="info_numbers">19.12</p>
                <span className="info_description">Дата жеребьёвки</span>
            </div>
            <div className="info_window">
                <p className="info_numbers">31.12</p>
                <span className="info_description">Отправить подарки до</span>
            </div>
            <div className="info_window">
                <p className="info_numbers">4</p>
                <span className="info_description">Кол-во участников</span>
            </div>
        </section>
        <section className="container_link">
            <p className="invite_link_text">Пригласительная ссылка</p>
            <div className="invite_link">
                <p>http://secret_santa/id=jdhgfdkgdfc15cfgsjfhgs1b</p>
            </div>
        </section>
        <section className="container_members">
            <div className="member">
                <div className="member_1"></div>
                <span>участ. 1</span>
            </div>
            <div className="member">
                <div className="member_2"></div>
                <span>участ. 2</span>
            </div>
            <div className="member">
                <div className="member_3"></div>
                <span>участ. 3</span>
            </div>
            <div className="member">
                <div className="member_4"></div>
                <span>участ. 4</span>
            </div>
        </section>
    </div>
    )
};

export default GameWasCreatedWithMembers;