import { RemsNGUIPage } from './app.po';

describe('rems-ng-ui App', () => {
  let page: RemsNGUIPage;

  beforeEach(() => {
    page = new RemsNGUIPage();
  });

  it('should display welcome message', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('Welcome to app!');
  });
});
