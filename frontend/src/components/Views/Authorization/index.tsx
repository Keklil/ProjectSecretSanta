import React, { useState } from 'react';
import { useForm, SubmitHandler } from 'react-hook-form';
import Loader from '../../Loader';
import './style.css';

interface IAuthorizationProps {
  handleAuth(login: string, password: string): Promise<boolean>;
}

interface IAuthorizationForm {
  login: string;
  password: string;
}

const Authorization: React.FC<IAuthorizationProps> = ({ handleAuth }) => {
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<IAuthorizationForm>({ mode: 'onBlur' });
  const [authPending, setAuthPending] = useState(false);

  const submit: SubmitHandler<IAuthorizationForm> = ({ login, password }) => {
    setAuthPending(true);
    handleAuth(login, password).then((res) => {
      if (!res) setAuthPending(false);
    });
  };

  return (
    <div className="authorization_wrapper">
      <div className="authorization_box">
        <h1 className="authorization_title">Авторизация</h1>
        <form className="" onSubmit={handleSubmit(submit)}>
          <section className="constituents_of_authorization">
            <div className="login_components">
              <label className="block login">
                Логин
                <input className="input_text" placeholder="Введите логин" {...register('login', { required: true })} />
              </label>
              {!!errors.login && <span className="default__error">Обязательное поле</span>}
            </div>
            <div className="password_components">
              <label className="block password">
                Пароль
                <input className="input_text" type="password" placeholder="Введите пароль" {...register('password', { required: true })} />
              </label>
              {!!errors.password && <span className="default__error">Обязательное поле</span>}
            </div>
            <div className="btn_join">
              <button type="submit" className="default__btn auth__log_in">
                Войти в систему
              </button>
            </div>
          </section>
        </form>
        {authPending && (
          <div>
            <Loader message="Авторизация..." />
          </div>
        )}
      </div>
    </div>
  );
};

export default Authorization;
