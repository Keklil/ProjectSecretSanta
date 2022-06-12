import React, { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
// import { getEvents } from '../../../services/ServerController';
import cardImage from '../../../assets/snowman.png';

import './style.css';

const ReadyGames: React.FC<IDefaultAdminProps> = ({ serverController }) => {
  const [events, setEvents] = useState([]);
  const nav = useNavigate();

  const loadEvents = async () => {
    const events = await serverController.getEvents();
    console.log(events.response);
    setEvents(events.response);
  };

  const clickHandler = (e: React.MouseEvent, item: any) => {
    if (e.ctrlKey) nav(`../game/${item.id}`);
    else nav(`../admin/editGame/${item.id}`);
  };

  useEffect(() => {
    loadEvents();
  }, []);

  return (
    <>
      <div className="rectangle">
        <h1 className="games">Игры</h1>
        <Link to="/admin/createGame" className="default__btn ready_games__create_button">
          создать
        </Link>
      </div>
      <div className="container">
        {events.map((item: any) => (
          <div key={item.id} className="default_game_card" onClick={(e) => clickHandler(e, item)}>
            <img className="default_game_card__img" src={cardImage} />
            <span className="default_game_card__title">{item.description}</span>
          </div>
        ))}
      </div>
    </>
  );
};

export default ReadyGames;
