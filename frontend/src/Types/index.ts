type ISetState<T> = React.Dispatch<React.SetStateAction<T>>;

interface IEvent {
  id?: string;
  description: string;
  endRegistration: string;
  endEvent: string;
  sumPrice?: number;
  sendFriends?: boolean;
  tracking?: boolean;
  reshuffle?: boolean;
  membersCount: number;
  memberView: any[];
  preference?: string;
}

type IExistEvent = Required<IEvent>;

interface IPreferences {
  apartment?: string;
  city?: string;
  name?: string;
  phoneNumber?: string;
  preference?: string;
  region?: string;
  street?: string;
  zip?: string;
}

type IExistPreferences = Required<IPreferences>;

interface IRecipientInfo {
  address: string;
  name: string;
  preferences: string;
}

interface IWishes {
  name: string;
  phoneNumber: string;
  zip: string;
  region: string;
  city: string;
  street: string;
  apartment: string;
  wish: string;
}

interface IUser {
  email: string;
  // exp: number;
  role: UserRole;
  token: string;
  UserID: string;
  name?: string;
  surname?: string;
  patronymic?: string;
  activeEvent?: IExistEvent;
}

type UserRole = 'admin' | 'user';

interface IServerController {
  getEvent(eventId: string): Promise<any>;
  setUserParams(user: IUser): void;
  saveEvent(event: IEvent): Promise<any>;
  deleteEvent(eventId: string): Promise<any>;
  editEvent(newEvent: IExistEvent): Promise<any>;
  getEvents(memberId?: string): Promise<any>;
  login(userName: string, password: string): Promise<any>;
  getMemberWishes(memberId: string, eventId: string): Promise<any>;
  saveMemberWishes(memberId: string, eventId: string, wishes: IWishes): Promise<any>;
  editMemberWishes(memberId: string, eventId: string, wishes: IWishes): Promise<any>;
  getUserInfo(userId: string): Promise<any>;
  getUserEvent(eventId: string): Promise<any>;
  getUserPreferences(eventId: string): Promise<any>;
  updateUserPreferences(eventId: string, preferences: IExistPreferences): Promise<any>;
  saveUserPreferences(eventId: string, preferences: IExistPreferences): Promise<any>;
  getRecipientInfo(eventId: string): Promise<any>;
  exitGame(eventId: string): Promise<any>;
}

interface IDefaultProps {
  serverController: IServerController;
  user: IUser;
  setUser?: ISetState<IUser | undefined>;
}

interface IDefaultAdminProps extends IDefaultProps {}
