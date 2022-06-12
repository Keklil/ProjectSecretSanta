import React, { FC } from 'react';
import { Link } from 'react-router-dom';
import './style.css';

const Home: FC = () => {
  return (
    <div className="home_view">
      <span>Тайный санта</span>
      <Link className="default__btn home_view__btn_create" to="/admin/createGame">
        создать
      </Link>
      <Link className="default__btn home_view__btn_games" to="/admin/games">
        игры
      </Link>
    </div>
  );
};

export default Home;
