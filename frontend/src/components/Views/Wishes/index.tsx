import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useForm, SubmitHandler } from 'react-hook-form';
// import { getEvent } from '../../../services/ServerController';
import './style.css';
import Loader from '../../Loader';

interface IWishesForm extends IExistPreferences {}

interface IGetUserPreferencesResponse {
  // state: UserPreferencesState;
  message: string;
  wishes: IPreferences;
}

type UserPreferencesState = 'some';

const Wishes: React.FC<IDefaultProps> = ({ serverController, user }) => {
  const { id } = useParams();
  const [preferences, setPreferences] = useState<IPreferences>();
  const [preferencesExist, setPreferencesExist] = useState(true);
  const [gameData, setGameData] = useState<IExistEvent>();
  const [eventExist, setEventExist] = useState(true);
  const [isAdmin] = useState(user.role === 'admin');
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<IWishesForm>({ mode: 'onBlur' });
  const nav = useNavigate();

  const submit: SubmitHandler<IWishesForm> = async (data) => {
    if (!id) throw new Error('Event not exist');
    console.log(preferencesExist);
    if (preferencesExist) {
      const preferencesEdited = await serverController.updateUserPreferences(id, data);
      console.log(preferencesEdited);
      if (!preferencesEdited.ok) return alert(`Ошибка при изменении. ${preferencesEdited.response?.message}`);
      // console.log(res);
    } else {
      const preferencesSaved = await serverController.saveUserPreferences(id, data);
      console.log(preferencesSaved);
    }
    nav(`../game/${id}`);
    console.log(data);
  };

  const loadGameData = async () => {
    if (!id) return setEventExist(false);
    if (isAdmin) {
      const event = await serverController.getEvent(id).catch(console.log);
      if (!event.ok) return setEventExist(false);
      console.log(event);
      setGameData(event.response as IExistEvent);
    } else {
      const preferences = await serverController.getUserPreferences(id).catch(console.log);
      if (!preferences.ok) {
        setPreferences({});
        return setPreferencesExist(false);
      }
      const { message, wishes } = preferences.response as IGetUserPreferencesResponse;
      if (message === 'Member joins for the first time') setPreferencesExist(false);
      setPreferences(wishes);
      // if (!event.ok) return nav(`../myWishes/${id}`);
      console.log(preferences);
      // if (!event.ok) return setEventExist(false);
      // setGameData(event.response as IExistEvent);
    }
  };

  useEffect(() => {
    loadGameData();
  }, []);
  // console.log(id);

  const saveWishes = () => {
    nav(`../game/${id}`);
  };

  return (
    <div className="wishes_content">
      {!eventExist && <div>Игра с ID {id} не найдена </div>}
      {!preferences && eventExist && <Loader />}
      {preferences && (
        <>
          <h2 className="wishes_content__title">Мое пожелание</h2>
          <form className="wishes_content__form" onSubmit={handleSubmit(submit)}>
            <div className="wishes_content__inputs_group">
              <div className="wishes_content__group_item">
                <label style={{ marginRight: '10%' }} className="default_input__label">
                  Имя
                  <br />
                  <input
                    defaultValue={preferences.name}
                    placeholder="Введите имя"
                    className="default_input"
                    {...register('name', { required: true })}
                  />
                </label>
              </div>
              <div className="wishes_content__group_item">
                <label className="default_input__label">
                  Номер телефона
                  <br />
                  <input
                    defaultValue={preferences.phoneNumber}
                    placeholder="Введите номер телефона"
                    className="default_input"
                    {...register('phoneNumber', { required: true })}
                  />
                </label>
              </div>
            </div>
            <div className="wishes_content__inputs_group">
              <div className="wishes_content__group_item">
                <label style={{ marginBottom: '5%' }} className="default_input__label">
                  Адрес проживания
                  <br />
                  <input
                    defaultValue={preferences.region}
                    style={{ width: '100%' }}
                    placeholder="Край/Область/Регион"
                    className="default_input"
                    {...register('region', { required: true })}
                  />
                </label>
              </div>
              <div className="wishes_content__group_item">
                <label style={{ marginBottom: '5%' }} className="default_input__label">
                  <br />
                  <input
                    defaultValue={preferences.city}
                    style={{ width: '100%' }}
                    placeholder="Город"
                    className="default_input"
                    {...register('city', { required: true })}
                  />
                </label>
              </div>
              <div className="wishes_content__group_item">
                <label style={{ marginBottom: '5%' }} className="default_input__label">
                  <br />
                  <input
                    defaultValue={preferences.street}
                    style={{ width: '100%' }}
                    placeholder="Адрес"
                    className="default_input"
                    {...register('street', { required: true })}
                  />
                </label>
              </div>
              <div className="wishes_content__group_item">
                <div className="wishes_content__inputs_group">
                  <div className="wishes_content__group_item">
                    <label style={{ marginBottom: '5%' }} className="default_input__label">
                      <br />
                      <input
                        defaultValue={preferences.apartment}
                        style={{ width: '100%' }}
                        placeholder="Квартира"
                        className="default_input"
                        {...register('apartment', { required: true })}
                      />
                    </label>
                  </div>
                  <div className="wishes_content__group_item">
                    <label style={{ marginBottom: '5%' }} className="default_input__label">
                      <br />
                      <input
                        defaultValue={preferences.zip}
                        style={{ width: '100%' }}
                        placeholder="Почтовый индекс"
                        className="default_input"
                        {...register('zip', { required: true })}
                      />
                    </label>
                  </div>
                </div>
              </div>
            </div>
            <div className="wishes_content__group_item">
              <label className="default_input__label">
                Пожелание по подарку
                <br />
                <textarea
                  defaultValue={preferences?.preference}
                  placeholder="Например, На новый год я хочу получить сертификат в спа-салон"
                  style={{ width: '100%', height: '100px', resize: 'none', padding: '15px 15px' }}
                  className="default_input"
                  {...register('preference', { required: true })}
                />
              </label>
            </div>
            <div className="wishes_content_save_btn_wrapper">
              <button type="submit" className="default__btn wishes_content_save_btn">
                Сохранить
              </button>
            </div>
          </form>
        </>
      )}
    </div>
  );
};

export default Wishes;
