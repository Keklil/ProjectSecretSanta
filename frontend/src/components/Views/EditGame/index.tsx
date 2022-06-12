import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useForm, SubmitHandler } from 'react-hook-form';
// import { saveEvent, getEvent, editEvent } from '../../../services/ServerController';
import './style.css';
import Loader from '../../Loader';

interface ICreateForm {
  description: string;
  endRegistration: string;
  endEvent: string;
  sumPrice?: number;
  showSumPrice: boolean;
}

const EditGame: React.FC<IDefaultAdminProps> = ({ serverController }) => {
  const [gameData, setGameData] = useState<IExistEvent>();
  const [eventExist, setEventExist] = useState(true);
  const { register, handleSubmit, watch } = useForm<ICreateForm>({ defaultValues: { showSumPrice: true } });
  const { id } = useParams();
  const showSumPrice = watch('showSumPrice');
  const nav = useNavigate();

  const submit: SubmitHandler<ICreateForm> = ({ description, endEvent, endRegistration, showSumPrice, sumPrice }) => {
    if (!id) {
      const event: IEvent = {
        description,
        endEvent,
        endRegistration,
        sumPrice: showSumPrice ? sumPrice : 0,
        membersCount: 0,
        memberView: [],
      };
      serverController
        .saveEvent(event)
        .then((res) => {
          if (!res.ok) throw new Error(res.error);
          alert('Save');
          return res.response.id;
        })
        .then((id) => nav(`../game/${id}`))
        .catch((error) => console.log(error));
    } else {
      const existEvent: IExistEvent = {
        id,
        description,
        endEvent,
        endRegistration,
        sumPrice: sumPrice ?? 0,
        reshuffle: false,
        sendFriends: false,
        tracking: false,
        membersCount: 0,
        memberView: [],
        preference: '',
      };
      serverController
        .editEvent(existEvent)
        .then(() => alert('edit'))
        .then(() => nav('../admin/games'));
    }
  };

  const loadGameData = async () => {
    if (!id) return setEventExist(false);
    const data = await serverController.getEvent(id);
    if (!data.ok) return setEventExist(false);
    const gameData = data.response as IExistEvent;
    setGameData(gameData);
    // console.log(new Date(gameData?.endRegistration).toLocaleDateString('ru', {}));
    // reset({
    //   description: gameData?.description,
    //   endEvent: gameData?.endEvent?.split('T')[0],
    //   endRegistration: gameData?.endRegistration?.split('T')[0],
    //   showSumPrice: !!gameData?.sumPrice,
    //   sumPrice: gameData?.sumPrice,
    // });
  };

  useEffect(() => {
    loadGameData();
  }, []);

  const deleteEvent = async (id: string) => {
    if (!confirm('Вы точно хотите удалить игру?')) return;
    const res = await serverController.deleteEvent(id).catch(console.log);
    console.log(res);
    nav('../admin/games');
  };

  const EditGameContent = (
    <>
      <h1 className="title">{id ? 'Изменение' : 'Создание'} игры</h1>
      <form className="edit_game__form" onSubmit={handleSubmit(submit)}>
        <div className="edit_game__form_sections">
          <div className="edit_game__form_section">
            <div className="edit_game__form_input">
              <label className="titles_inputs">
                Название игры
                <input
                  className="input_title_game"
                  type="text"
                  placeholder="Например, Тайный Санта 2022"
                  defaultValue={gameData?.description}
                  {...register('description', { required: true })}
                />
              </label>
            </div>
          </div>
          <div className="edit_game__form_section">
            <div className="edit_game__form_sections">
              <div className="edit_game__form_input">
                <label className="titles_inputs">
                  Дата жеребьевки
                  <input
                    className="input_date"
                    type="date"
                    defaultValue={gameData?.endRegistration?.split('T')[0]}
                    placeholder="дд/мм/гггг"
                    {...register('endRegistration', { required: true })}
                  />
                </label>
              </div>
              <div className="edit_game__form_input">
                <label className="titles_inputs">
                  Отправить подарок до
                  <input
                    className="input_date"
                    type="date"
                    defaultValue={gameData?.endEvent?.split('T')[0]}
                    placeholder="дд/мм/гггг"
                    {...register('endEvent', { required: true })}
                  />
                </label>
              </div>
            </div>
          </div>
          <div className="edit_game__form_section">
            <div className="edit_game__form_input" style={{ display: 'flex' }}>
              <div className="recommended_cost">
                <p className="title_recommended_cost">Рекомендуемая стоимость подарка</p>
                <span>При включении опции участникам смогут увидеть рекомендуемую стоимость, для получения подарка</span>
              </div>
              <label className="switch">
                <input type="checkbox" {...register('showSumPrice')} />
                <span className="slider_round"></span>
              </label>
            </div>
          </div>
          {showSumPrice && (
            <div className="gift_cost edit_game__form_input">
              <p className="title_gift_cost ">Сумма подарка</p>
              <input className="input_gift_cost" type="number" defaultValue={gameData?.sumPrice} {...register('sumPrice', { required: true })} />
              <select className="select_currency" name="currency" id="currency">
                <option className="currency">Руб</option>
              </select>
            </div>
          )}
        </div>
        {/* <section className="inputs_container"></section>
        <section className="recommended_cost_container"></section> */}
        <div className="edit_game__buttons">
          <button className="default__btn" type="submit">
            {id ? 'Сохранить изменения' : 'Создать игру'}
          </button>
          {!!id && (
            <>
              <button className="default__btn" onClick={() => nav(`../game/${id}`)}>
                Перейти к игре
              </button>
              <button className="default__btn" type="button" onClick={() => deleteEvent(id)}>
                Удалить
              </button>
            </>
          )}
        </div>
      </form>
    </>
  );

  return (
    <>
      {/* {!eventExist && <div>Bad</div>} */}
      {!gameData && eventExist && <Loader />}
      {(!!gameData || !eventExist) && EditGameContent}
    </>
  );
};

export default EditGame;
