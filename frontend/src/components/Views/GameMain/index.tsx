import React, { useEffect, useState } from 'react';
import playerIcon from '../../../assets/playerIcon.png';
import { useParams, useNavigate } from 'react-router-dom';
// import { getEvent } from '../../../services/ServerController';
import './style.css';
import FindMember from '../FindMember';
import DesignatedUser from '../DesignatedUser';
import Loader from '../../Loader';

const GameMain: React.FC<IDefaultProps> = ({ serverController, user, setUser }) => {
  const { id } = useParams();
  const [gameData, setGameData] = useState<IExistEvent>();
  const [isReshuffle, setIsReshuffle] = useState(false);
  const [eventExist, setEventExist] = useState(true);
  const [isAdmin] = useState(user.role === 'admin');

  const nav = useNavigate();

  const loadGameData = async () => {
    if (!id) return setEventExist(false);
    if (isAdmin) {
      const event = await serverController.getEvent(id).catch(console.log);
      if (!event.ok) return setEventExist(false);
      console.log(event);
      setGameData(event.response as IExistEvent);
    } else {
      const event = await serverController.getUserEvent(id).catch(console.log);
      if (!event.ok) return nav(`../myWishes/${id}`);
      console.log(event);
      const existEvent = event.response as IExistEvent;
      existEvent.id = id;
      setUser?.((x) => (x ? { ...x, activeEvent: existEvent } : undefined));
      // if (!event.ok) return setEventExist(false);
      setGameData(existEvent);
    }
  };

  useEffect(() => {
    if (!gameData) return;
    // gameData.reshuffle = true;
    setIsReshuffle(gameData.reshuffle);
    // if (gameData?.reshuffle) console.log('прошло');
  }, [gameData]);

  useEffect(() => {
    loadGameData();
  }, []);

  const GameView = (
    <>
      <div className="info_card__group">
        <div className="default_info_card">
          <div className="default_info_card__title">{gameData?.sumPrice ?? 0}</div>
          <div className="default_info_card__description">Рекомендуемая стоимость</div>
        </div>
        <div className="default_info_card">
          <div className="default_info_card__title">{new Date(String(gameData?.endRegistration)).toLocaleDateString()}</div>
          <div className="default_info_card__description">Дата жеребьевки</div>
        </div>
        <div className="default_info_card">
          <div className="default_info_card__title">{new Date(String(gameData?.endEvent)).toLocaleDateString()}</div>
          <div className="default_info_card__description">Отправить подарки до</div>
        </div>
        <div className="default_info_card">
          <div className="default_info_card__title">{gameData?.membersCount ?? 0}</div>
          <div className="default_info_card__description">Кол-во участников</div>
        </div>
      </div>
      <div className="game_main_content__my_wishes">
        <div className="default_input game_main_content__my_wishes_text">
          {user.role === 'user' ? gameData?.preference : `http://localhost:3000/joinGame/${id}`}
        </div>
      </div>
      {user.role === 'user' && (
        <div className="game_main_content__edit_wishes_btn">
          <button onClick={() => nav(`../myWishes/${id}`)} className="default__btn">
            Редактировать
          </button>
        </div>
      )}
      <div className="game_main_content__players">
        {gameData?.memberView?.map((item, index) => {
          const { memberView } = item;
          return (
            <div key={index} className="default_player_card">
              <div className="default_player_card__image">
                <img src={playerIcon} />
              </div>
              <div className="default_player_card__name">{memberView.name}</div>
            </div>
          );
        })}
      </div>
    </>
  );
  // console.log(gameData);
  // console.log(isReshuffle);
  // console.log(isAdmin);
  // console.log(id);
  return (
    <>
      {!eventExist && <div>Bad</div>}
      {!gameData && eventExist && <Loader />}
      {!isReshuffle && !!gameData && GameView}
      {isReshuffle && !!gameData && isAdmin && <FindMember serverController={serverController} user={user} gameData={gameData} />}
      {isReshuffle && !!gameData && !isAdmin && <DesignatedUser serverController={serverController} user={user} />}
    </>
  );
};

export default GameMain;
