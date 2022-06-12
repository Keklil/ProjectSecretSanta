import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import Loader from '../../Loader';
// import { getEvent } from '../../../services/ServerController';

import './style.css';

const Main: React.FC<IDefaultProps> = ({ serverController, user }) => {
  const { id } = useParams();
  const [gameData, setGameData] = useState<IExistEvent>();
  const [eventExist, setEventExist] = useState(true);
  const [isMemberGame, setIsMemberGame] = useState(true);
  const nav = useNavigate();

  const loadGameData = async () => {
    if (!id) return setEventExist(false);
    if (user.role === 'admin') {
      const event = await serverController.getEvent(id).catch(console.log);
      if (!event.ok) return setEventExist(false);
      console.log(event);
      setGameData(event.response as IExistEvent);
    } else if (user.role === 'user') {
      const event = await serverController.getUserEvent(id).catch(console.log);
      // if (!event.ok) return nav(`../myWishes/${id}`);
      console.log(event);
      if (!event.ok) return setIsMemberGame(false);
      setGameData(event.response as IExistEvent);
    }
  };

  useEffect(() => {
    loadGameData();
  }, []);

  const acceptGame = () => {
    isMemberGame ? nav(`../game/${id}`) : nav(`../myWishes/${id}`);
  };

  return (
    <div className="main_view">
      <span className="main_view__title">Тайный санта</span>
      <div className="main_view__info">
        {!id && <div>Чтобы войти в игру, перейдите по пригласительной ссылке</div>}
        {!eventExist && !!id && <div>Игра с ID {id} не найдена </div>}
        {isMemberGame && !gameData && eventExist && <Loader />}
        {!isMemberGame && (
          <>
            <span className="main_view__info_text">
              Вас пригласили участвовать <br />в игре
            </span>
            <button style={{ marginTop: '3%' }} className="default__btn" onClick={acceptGame}>
              принять приглашение
            </button>
          </>
        )}
        {isMemberGame && !!gameData && (
          <>
            <span className="main_view__info_text">
              Вы уже участник <br />
              игры “{gameData.description}”
            </span>
            <button style={{ marginTop: '3%' }} className="default__btn accept" onClick={acceptGame}>
              Перейти к игре
            </button>
          </>
        )}
        {/* <button className="default__btn">перейти к игре</button> */}
      </div>
    </div>
  );
};

export default Main;
